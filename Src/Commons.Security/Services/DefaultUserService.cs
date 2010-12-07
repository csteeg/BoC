using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Transactions;
using System.Web.Configuration;
using BoC.EventAggregator;
using BoC.Extensions;
using BoC.Persistence;
using BoC.Security.Model;
using BoC.Services;
using BoC.Validation;

namespace BoC.Security.Services
{
	public class DefaultUserService : BaseModelService<User>, IUserService
	{
		private const Int32 SALT_BYTES = 16;
		private readonly IRepository<Role> roleRepository;
		private readonly IModelValidator modelValidator;
		private readonly IRepository<User> userRepository;

		public DefaultUserService(IEventAggregator eventAggregator, IRepository<User> userRepository, IRepository<Role> roleRepository, IModelValidator modelValidator) : 
			base(modelValidator, eventAggregator, userRepository)
		{
			this.userRepository = userRepository;
			this.roleRepository = roleRepository;
			this.modelValidator = modelValidator;

			//Defuault settings:
			MinRequiredPasswordLength = 5;
			MinRequiredNonAlphanumericCharacters = 0;
			PasswordFormat = PasswordFormat.Hashed;
			PasswordAttemptWindowMinutes = 10;
			MaxInvalidPasswordAttempts = 10;
			RequiresUniqueEmail = false;
			RequiresApproval = false;
		}

		#region IUserService Settings

		public bool RequiresApproval { get; set; }
		public bool RequiresUniqueEmail { get; set; }
		public int MaxInvalidPasswordAttempts { get; set; }
		public int PasswordAttemptWindowMinutes { get; set; }
		public PasswordFormat PasswordFormat { get; set; }
		public int MinRequiredNonAlphanumericCharacters { get; set; }
		public int MinRequiredPasswordLength { get; set; }
		public string PasswordStrengthRegularExpression { get; set; }

		#endregion
		public virtual Boolean UserExists(String login)
		{
			if (String.IsNullOrEmpty(login))
			{
				return false;
			}
			return (
					   from u in userRepository.Query()
					   where u.Login == login
					   select u
				   ).Count() > 0;
		}

		public virtual Boolean ChangePassword(User user, String oldPassword, String newPassword)
		{
			if (user == null)
			{
				return false;
			}

			Boolean result = false;
			using (var scope = new TransactionScope())
			{
				if (user.ChangePassword(oldPassword, newPassword))
				{
					userRepository.SaveOrUpdate(user);
					result = true;
				}

				scope.Complete();
			}
			return result;
		}

		public virtual void SetPassword(User user, string password)
		{
			if (user == null)
			{
				throw new UserNotFoundException();
			}

			using (var scope = new TransactionScope())
			{
				user.Password = EncodePassword(password);
				user.LastPasswordChange = DateTime.Now;

				userRepository.SaveOrUpdate(user);
				scope.Complete();
			}
		}

		public override User Insert(User user)
		{
			return Insert(user, null);
		}

		public virtual User Insert(User user, string password)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (RequiresUniqueEmail && !String.IsNullOrEmpty(user.Email) && FindByEmail(user.Email) != null)
			{
				throw new EmailInUseException(user.Email);
			}
			if (FindByLogin(user.Login) != null)
			{
				throw new LoginInUseException(user.Login);
			}
			if (password != null)
			{
				user.Password = EncodePassword(password);
			}
			user.IsApproved = user.IsApproved || RequiresApproval;

			ValidateEntity(user);
			
			using (var scope = new TransactionScope())
			{
				user = base.Insert(user);
				scope.Complete();
			}
			return user;
		}

		public override User SaveOrUpdate(User user)
		{
			if (!String.IsNullOrEmpty(user.Email) && RequiresUniqueEmail)
			{
				var otheruser = FindByEmail(user.Email);
				if (otheruser != null && otheruser.Id != user.Id)
				{
					throw new EmailInUseException(user.Email);
				}
			}

			if (!String.IsNullOrEmpty(user.Login))
			{
				var otheruser = FindByLogin(user.Login);
				if (otheruser != null && otheruser.Id != user.Id)
				{
					throw new LoginInUseException(user.Login);
				}
			}

			return user.Id <= 0 ? 
				Insert(user) 
				: base.SaveOrUpdate(user);
		}

		public virtual void DeleteUser(User user)
		{
			if (user == null)
			{
				throw new UserNotFoundException();
			}

			using (var scope = new TransactionScope())
			{
				userRepository.Delete(user);
				scope.Complete();
			}
		}

		public virtual Int32 CountOnlineUsers(TimeSpan activitySpan)
		{
			DateTime compareTime = DateTime.Now.Subtract(activitySpan);
			return
				(from u in userRepository.Query()
				 where u.LastActivity > compareTime
				 select u).Count();
		}

		public virtual void UnlockUser(User user)
		{
			using (var scope = new TransactionScope())
			{
				if (user == null)
				{
					throw new UserNotFoundException();
				}
				else
				{
					user.IsLockedOut = false;
					user.LastLockedOut = DateTime.Now;

					userRepository.SaveOrUpdate(user);
				}
				scope.Complete();
			}
		}

		public virtual void LockUser(User user)
		{
			using (var scope = new TransactionScope())
			{
				user.IsLockedOut = true;
				user.LastLockedOut = DateTime.Now;

				userRepository.SaveOrUpdate(user);
				scope.Complete();
			}
		}

		public virtual User FindByEmail(string email)
		{
			return userRepository.Query().Where(u => u.Email == email).FirstOrDefault();
		}

		public virtual User FindByLogin(string login)
		{
			if (string.IsNullOrEmpty(login))
				return null;
			
			return userRepository.Query().Where(u => u.Login == login).FirstOrDefault();
		}
		
		public virtual IEnumerable<User> FindUsersByPartialLogin(String login)
		{
			return from u in userRepository.Query()
					where u.Login.Contains(login)
					select u;
		}

		public virtual Int32 CountUsersByPartialLogin(String login)
		{
			return (from u in userRepository.Query()
					where u.Login.Contains(login)
					select u).Count();
		}

		public virtual IEnumerable<User> FindUsersByPartialEmail(String email)
		{
			return from u in userRepository.Query()
					where u.Email.Contains(email)
					select u;
		}

		public virtual Int32 CountUsersByPartialEmail(String email)
		{
			return (from u in userRepository.Query()
					where u.Email.Contains(email)
					select u).Count();
		}

		public virtual User Authenticate(String login, String password)
		{
			if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
				return null;

			User user = null;
			using (var scope = new TransactionScope())
			{
				//password = EncodePassword(password);
				user = userRepository.Query().Where(u =>
									u.Login == login &&
									u.IsApproved && !u.IsLockedOut
					).FirstOrDefault();

				if (user != null)
				{
					if (!CheckPassword(password, user.Password))
					{
						UpdateFailureCount(user);
						user = null;
					}
					else
					{
						user.LastLogin = DateTime.Now;
						userRepository.Update(user);
					}
				}
				scope.Complete();
			}
			if (user != null)
			{
				return user;
			}
			else
			{
				throw new RulesException("Login", "Incorrect username or password");
			}
		}

		#region roles

		public virtual void CreateRole(String roleName)
		{
			if (!RoleExists(roleName))
			{
				roleRepository.SaveOrUpdate(
					new Role(roleName));
			}
			else
			{
				new RoleExistsException(roleName);
			}
		}

		public virtual Boolean RoleExists(String roleName)
		{
			return (FindRole(roleName) != null);
		}

		public virtual Role GetRole(long id)
		{
			return roleRepository.Get(id);
		}

		public virtual Role FindRole(String roleName)
		{
			return
				(from role in roleRepository.Query()
				where role.RoleName == roleName
				select role).FirstOrDefault();
		}

		public virtual void DeleteRole(String roleName)
		{
			Role role = FindRole(roleName);
			if (role == null)
			{
				throw new RoleNotFoundException(roleName);
			}
			else
			{
				DeleteRole(role, false);
			}
		}

		public virtual void DeleteRole(Role role)
		{
			if (role == null)
			{
				throw new RoleNotFoundException();
			}

			DeleteRole(role, false);
		}

		public virtual void DeleteRole(String roleName, Boolean throwWhenUsed)
		{
			Role role = FindRole(roleName);
			if (role == null)
			{
				throw new RoleNotFoundException(roleName);
			}
			else
			{
				DeleteRole(role, throwWhenUsed);
			}
		}

		public virtual void DeleteRole(Role role, Boolean throwWhenUsed)
		{
			if (role == null)
			{
				throw new RoleNotFoundException();
			}

			//TODO: optimize instead of role.users.count
			if (throwWhenUsed && role.Users.Count > 0)
			{
				throw new RoleInUseException(role.RoleName);
			}

			roleRepository.Delete(role);
		}


		public User GetByPrincipal(IPrincipal principal)
		{
			if (principal is User)
				return principal as User;

			if (principal == null || !principal.Identity.IsAuthenticated || String.IsNullOrEmpty(principal.Identity.Name))
			{
				return null;
			}

			long userid;
			if (long.TryParse(principal.Identity.Name, out userid))
			{
				return Get(userid);
			}
			return null;
		}

		public virtual void AddUsersToRoles(IEnumerable<User> users, IEnumerable<Role> roles)
		{
			if ((users == null) || (roles == null))
			{
				return;
			}

			using (var scope = new TransactionScope())
			{
				foreach (User u in users)
				{
					foreach (Role role in roles)
					{
						u.Roles.Add(role);
					}
					userRepository.SaveOrUpdate(u);
				}
				scope.Complete();
			}
		}

		public virtual void AddUsersToRoles(IEnumerable<String> logins, IEnumerable<String> roleNames)
		{
			AddUsersToRoles(
				from user in userRepository.Query()
				where logins.Contains(user.Login)
				select user, 
				from role in roleRepository.Query()
				where roleNames.Contains(role.RoleName)
				select role);
		}

		public virtual void RemoveUsersFromRoles(IEnumerable<String> logins, IEnumerable<String> roleNames)
		{
			RemoveUsersFromRoles(
				from user in userRepository.Query()
				where logins.Contains(user.Login)
				select user,
				from role in roleRepository.Query()
				where roleNames.Contains(role.RoleName)
				select role);
		}

		public virtual void RemoveUsersFromRoles(IEnumerable<User> users, IEnumerable<Role> roles)
		{
			if ((users == null) || (roles == null))
			{
				return;
			}

			using (var scope = new TransactionScope())
			{
				foreach (User u in users)
				{
					foreach (Role role in roles)
					{
						u.Roles.Remove(role);
					}
					userRepository.SaveOrUpdate(u);
				}
				scope.Complete();
			}
		}

		public virtual Role[] GetAllRoles()
		{
			return roleRepository.Query().ToArray();
		}

		#endregion

		//
		// EncodePassword
		//   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
		//
		public virtual String EncodePassword(String password)
		{
			if (password == null)
			{
				return null;
			}

			String encodedPassword = password;
			switch (PasswordFormat)
			{
				case PasswordFormat.Clear:
					break;
				case PasswordFormat.Encrypted:
					encodedPassword =
						Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
					break;
				case PasswordFormat.Hashed:
					encodedPassword = password.MD5();
					break;
				default:
					throw new NotImplementedException("Unsupported password format.");
			}

			return encodedPassword;
		}


		//
		// UnEncodePassword
		//   Decrypts or leaves the password clear based on the PasswordFormat.
		//
		private String UnEncodePassword(String encodedPassword)
		{
			String password = encodedPassword;

			switch (PasswordFormat)
			{
				case PasswordFormat.Clear:
					break;
				case PasswordFormat.Encrypted:
					password =
						Encoding.Unicode.GetString(
							DecryptPassword(Convert.FromBase64String(password))
							);
					break;
				case PasswordFormat.Hashed:
					throw new UserServiceException("Password", "Cannot unencode a hashed password.");
				default:
					throw new NotImplementedException("Unsupported password format.");
			}

			return password;
		}

		private SymmetricAlgorithm GetAlg(out byte[] decryptionKey)
		{
			var section = (MachineKeySection) WebConfigurationManager.GetSection("system.web/machineKey");

			if (section.DecryptionKey.StartsWith("AutoGenerate"))
			{
				throw new UserServiceException("Password",
					"You must explicitly specify a decryption key in the <machineKey> section when using encrypted passwords.");
			}

			String alg_type = section.Decryption;
			if (alg_type == "Auto")
			{
				alg_type = "AES";
			}

			SymmetricAlgorithm alg = null;
			if (alg_type == "AES")
			{
				alg = Rijndael.Create();
			}
			else if (alg_type == "3DES")
			{
				alg = TripleDES.Create();
			}
			else
			{
				throw new UserServiceException("Password",
					String.Format("Unsupported decryption attribute '{0}' in <machineKey> configuration section",
								  alg_type));
			}

			decryptionKey = Encoding.Unicode.GetBytes(section.DecryptionKey);
			return alg;
		}

		protected byte[] DecryptPassword(byte[] encodedPassword)
		{
			byte[] decryptionKey;

			using (SymmetricAlgorithm alg = GetAlg(out decryptionKey))
			{
				alg.Key = decryptionKey;

				using (ICryptoTransform decryptor = alg.CreateDecryptor())
				{
					byte[] buf = decryptor.TransformFinalBlock(encodedPassword, 0, encodedPassword.Length);
					var rv = new byte[buf.Length - SALT_BYTES];

					Array.Copy(buf, 16, rv, 0, buf.Length - 16);
					return rv;
				}
			}
		}

		protected byte[] EncryptPassword(byte[] password)
		{
			byte[] decryptionKey;
			var iv = new byte[SALT_BYTES];

			Array.Copy(password, 0, iv, 0, SALT_BYTES);
			Array.Clear(password, 0, SALT_BYTES);

			using (SymmetricAlgorithm alg = GetAlg(out decryptionKey))
			{
				using (ICryptoTransform encryptor = alg.CreateEncryptor(decryptionKey, iv))
				{
					return encryptor.TransformFinalBlock(password, 0, password.Length);
				}
			}
		}

		//
		// UpdateFailureCount
		//  
		/// <summary>
		/// A helper method that performs the checks and updates associated with
		/// password failure tracking.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="isPasswordAnswer">true if the passwordanswer has failed, false if password was incorrect</param>
		protected void UpdateFailureCount(User user)
		{
			var windowStart = new DateTime();
			Int32 failureCount = 0;

			if (user == null)
			{
				throw new UserNotFoundException();
			}

			failureCount = user.FailedPasswordAttemptCount;
			windowStart = user.FailedPasswordAttemptWindowStart.HasValue
							  ? user.FailedPasswordAttemptWindowStart.Value
							  : DateTime.MinValue;

			DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindowMinutes);

			using (var scope = new TransactionScope())
			{
				if (failureCount == 0 || DateTime.Now > windowEnd)
				{
					// First password failure or outside of PasswordAttemptWindow. 
					// Start a new password failure count from 1 and a new window starting now.
					user.FailedPasswordAttemptCount = 1;
					user.FailedPasswordAttemptWindowStart = DateTime.Now;
				}
				else
				{
					if (++failureCount >= MaxInvalidPasswordAttempts && MaxInvalidPasswordAttempts > 0)
					{
						// Password attempts have exceeded the failure threshold. Lock out
						// the user.
						user.IsLockedOut = true;
						user.LastLockedOut = DateTime.Now;
					}
					else
					{
						// Password attempts have not exceeded the failure threshold. Update
						// the failure counts. Leave the window the same.
						user.FailedPasswordAttemptCount = failureCount;
					}
				}

				userRepository.SaveOrUpdate(user);
				scope.Complete();
			}
		}

		//
		// CheckPassword
		//   Compares password values based on the MembershipPasswordFormat.
		//
		public virtual Boolean CheckPassword(String password, String dbpassword)
		{
			switch (PasswordFormat)
			{
				case PasswordFormat.Encrypted:
					dbpassword = UnEncodePassword(dbpassword);
					break;
				case PasswordFormat.Hashed:
					password = EncodePassword(password);
					break;
				default:
					break;
			}

			return (password == dbpassword);
		}
	}
}