﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using BoC.Logging;
using Glass.Mapper.Configuration;
using Glass.Mapper.Sc.Configuration;
using Lucene.Net.Search;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Search;
using Sitecore.ContentSearch;
using BuiltinFields = Sitecore.Search.BuiltinFields;

namespace BoC.Persistence.SitecoreGlass
{

    public class SitecoreRepository<T> : IRepository<T> where T : class, IBaseEntity<Guid>, new()
    {
        protected IDatabaseProvider _dbProvider;
        protected ISitecoreServiceProvider _sitecoreServiceProvider;
        private readonly IProviderSearchContextProvider _searchContextProvider;
        private readonly ILogger _logger;
        private ParentConfiguration _parentProperty;
        private SitecoreInfoConfiguration _pathProperty;
        private SitecoreInfoConfiguration _nameProperty;
        private SitecoreInfoConfiguration _templateIdsProperty;
        private SitecoreInfoConfiguration _languageProperty;

        public SitecoreRepository(  IDatabaseProvider dbProvider, 
                                    ISitecoreServiceProvider sitecoreServiceProvider,
                                    IProviderSearchContextProvider searchContextProvider,
                                    ILogger logger)
        {
            _dbProvider = dbProvider;
            _sitecoreServiceProvider = sitecoreServiceProvider;
            _searchContextProvider = searchContextProvider;
            _logger = logger;

        }

        protected SitecoreInfoConfiguration PathProperty
        {
            get { return _pathProperty ?? (_pathProperty = _sitecoreServiceProvider.GetSitecoreService().GlassContext[typeof(T)].Properties.OfType<SitecoreInfoConfiguration>().FirstOrDefault(p => p.Type == SitecoreInfoType.FullPath || p.Type == SitecoreInfoType.Path)); }
        }

        protected ParentConfiguration ParentProperty
        {
            get { return _parentProperty ?? (_parentProperty = _sitecoreServiceProvider.GetSitecoreService().GlassContext[typeof(T)].Properties.OfType<ParentConfiguration>().FirstOrDefault()); }
        }

        protected SitecoreInfoConfiguration NameProperty
        {
            get { return _nameProperty ?? (_nameProperty = _sitecoreServiceProvider.GetSitecoreService().GlassContext[typeof(T)].Properties.OfType<SitecoreInfoConfiguration>().FirstOrDefault(p => p.Type == SitecoreInfoType.Name)); }
        }

        protected SitecoreInfoConfiguration TemplateIdsProperty
        {
            get { return _templateIdsProperty ?? (_templateIdsProperty = _sitecoreServiceProvider.GetSitecoreService().GlassContext[typeof(T)].Properties.OfType<SitecoreInfoConfiguration>().FirstOrDefault(p => p.Type == SitecoreInfoType.BaseTemplateIds)); }
        }

        protected SitecoreInfoConfiguration LanguageProperty
        {
            get { return _languageProperty ?? (_languageProperty = _sitecoreServiceProvider.GetSitecoreService().GlassContext[typeof(T)].Properties.OfType<SitecoreInfoConfiguration>().FirstOrDefault(p => p.Type == SitecoreInfoType.Language)); }
        }

        private IBaseEntity<Guid> GetParent(T model)
        {
            if (model == null)
                return null;

            if (ParentProperty != null)
            {
                return ParentProperty.PropertyInfo.GetValue(model, null) as IBaseEntity<Guid>;
            }

            if (model.Id != default(Guid))
            {
                var item = _dbProvider.GetDatabase().GetItem(new ID(model.Id));
                if (item != null && item.Parent != null)
                {
                    var result = ConvertItem<IBaseEntity<Guid>>(item.Parent);
                    if (result != null)
                        return result;
                }
            }
            if (PathProperty != null)
            {
                var path = (PathProperty.PropertyInfo.GetValue(model, null) + "").TrimEnd('/');
                path = path.Substring(0, path.LastIndexOf('/')) + "/";
                if (path != "/")
                {
                    var parent = GetAndConvertItem<IBaseEntity<Guid>>(path, GetLanguage(_dbProvider));
                    if (parent != null)
                        return parent;
                }
            }
            return null;
        }

        private string GetSitecoreName(IBaseEntity<Guid> model)
        {
            if (model == null)
                return null;
            if (NameProperty != null)
                return NameProperty.PropertyInfo.GetValue(model, null) as string;
            if (model.Id != default(Guid))
            {
                var item = _dbProvider.GetDatabase().GetItem(new ID(model.Id));
                return item == null ? null : item.Name;
            }
            return null;
        }

        private string GetPath(IBaseEntity<Guid> model)
        {
            if (model == null)
                return null;
            if (PathProperty != null)
                return PathProperty.PropertyInfo.GetValue(model, null) + "";
            if (model.Id != default(Guid))
            {
                var item = _dbProvider.GetDatabase().GetItem(new ID(model.Id));
                return item == null ? null : item.Paths.FullPath;
            }
            return null;
        }

        public virtual T SaveOrUpdate(T model)
        {
            var language = GetLanguage(_dbProvider);
            var sitecoreService = _sitecoreServiceProvider.GetSitecoreService();
            using (new LanguageSwitcher(language))
            {
                var parent = GetParent(model);

                _logger.Debug(String.Format("Saving {0} (id: {1}, language: {2}, displayname: {3}", typeof(T), model.Id, language, model));
                if (model.Id == Guid.Empty)
                {
                    //try to find an existing item based on path: sitecore creates two items with the same name otherwise
                    var path = GetPath(model);
                    string parentPath;
                    string itemName;
                    if (String.IsNullOrEmpty(path) && 
                        parent != null &&
                        !String.IsNullOrEmpty(itemName = GetSitecoreName(model)) && 
                        !String.IsNullOrEmpty(parentPath = GetPath(parent)))
                    {
                        path = parentPath + "/" + ItemUtil.ProposeValidItemName(itemName);
                    }
                    var saved = false;
                    if (!String.IsNullOrEmpty(path))
                    {
                        var sitecoreItem = _dbProvider.GetDatabase().GetItem(path);
                        if (sitecoreItem != null)
                        {
                            model.Id = sitecoreItem.ID.ToGuid();
                            if (PathProperty != null)
                                PathProperty.PropertyInfo.SetValue(model, sitecoreItem.Paths.Path, null);
                            sitecoreService.Save(model);
                            saved = true;
                        }
                    }
                    if (!saved)
                    {
                        model = sitecoreService.Create(parent, model);
                    }
                }
                else
                {
                    sitecoreService.Save(model);
                }

                return model;
            }
        }

        public virtual T Get(string path)
        {
            return Get(path, GetLanguage(_dbProvider));
        }

        public virtual T Get(string path, Language language)
        {
            return GetAndConvertItem<T>(path, language);
        }

        public virtual T Get(Guid id)
        {
            var language = GetLanguage(_dbProvider);
            return Get(id, language);
        }

        public T Get(Guid id, Language language)
        {
            return GetAndConvertItem<T>(id, language);
        }

        private TargetClass GetAndConvertItem<TargetClass>(object id, Language language) where TargetClass: class
        {
            ID itemid = id is ID ? (ID)id : (id is Guid ? new ID((Guid)id) : ID.Null);
            if (itemid == ID.Null && ID.IsID(id + ""))
                itemid = new ID(id + "");

            if (language == null)
            {
                language = GetLanguage(_dbProvider);
            }
            
            using (new LanguageSwitcher(language))
            {
                var sitecoreItem = itemid == ID.Null ?
                        _dbProvider.GetDatabase().GetItem(id + "", language) :
                        _dbProvider.GetDatabase().GetItem(itemid, language);
                return ConvertItem<TargetClass>(sitecoreItem);
            }

        }

        private TargetClass ConvertItem<TargetClass>(Item sitecoreItem) where TargetClass : class
        {
            return sitecoreItem != null && sitecoreItem.Versions.Count > 0
                       ? _sitecoreServiceProvider.GetSitecoreService().CreateType<TargetClass>(sitecoreItem, true, true)
                       : null;
        }

        public Language GetLanguage(IDatabaseProvider dbProvider)
        {
            if (dbProvider.GetCulture() == null || dbProvider.GetCulture().Equals(CultureInfo.InvariantCulture))
            {
                try
                {
                    var a = global::Sitecore.Context.Language;
                } // TODO: for unittests: find out why Context.Language is loaded correctly the second time
                catch
                {
                }
                return global::Sitecore.Context.Language;
            }
            return LanguageManager.GetLanguage(dbProvider.GetCulture().Name);
        }

        public IEnumerable<T> GetItems(IEnumerable<ItemUri> itemUris)
        {
            var sitecoreService = _sitecoreServiceProvider.GetSitecoreService();
            foreach (var itemUri in itemUris)
            {
                var item = sitecoreService.Database.GetItem(itemUri.ItemID, itemUri.Language, itemUri.Version);
                if (item == null) yield return null;
                yield return sitecoreService.CreateType<T>(item, true, true);
            }

        }
        [Obsolete("THIS SHOULD BE MADE LUCENE-INDEPENDENT!")]
        public IEnumerable<T> Query(Query query, out int totalResults, Guid rootItemId = default(Guid), int start = 0,
                                    int count = int.MaxValue)
        {
            var sitecoreService = _sitecoreServiceProvider.GetSitecoreService();
            var config = sitecoreService.GlassContext.GetTypeConfiguration < SitecoreTypeConfiguration>(typeof(T));
            totalResults = 0;
            if (config == null)
                return null;
            var searchContext = new FixedSearchContext()
                {
                    TemplateID = config.TemplateId.ToGuid()
                };

            var boolQuery = new BooleanQuery();
            if (query != null)
                boolQuery.Add(query, Occur.MUST);
            if (rootItemId == (default(Guid)) && rootItemId != global::Sitecore.ItemIDs.RootID.ToGuid())
            {
                var rootItem = sitecoreService.Database.GetItem(ID.Parse(rootItemId));
                searchContext.Item = rootItem;
            }

            searchContext.ContentLanguage = GetLanguage(_dbProvider);
            using (var context = new IndexSearchContext(_searchContextProvider.GetProviderSearchContext().Index as ILuceneIndex))
            {
                var searchHits = context.Search(boolQuery, searchContext);

                totalResults = searchHits.Length;
                //var resultCollection = searchHits.FetchResults(start, count);
                var hits = (count == int.MaxValue) ? searchHits.Slice(start) : searchHits.Slice(start, count);
                return GetItems(
                    hits.Select(hit => hit.Document.GetField(BuiltinFields.Url))
                        .Where(uriField => uriField != null && !string.IsNullOrEmpty(uriField.StringValue))
                        .Select(uriField => ItemUri.Parse(uriField.StringValue))
                        .ToList());
            }
        }

        public void Dispose()
        {
            _sitecoreServiceProvider = null;
        }

        public void Delete(T model)
        {
            var language = GetLanguage(_dbProvider);
            _logger.Debug(String.Format("Deleting {0} (id: {1}, language: {2}, displayname: {3}", typeof (T), model.Id, language, model));
            using (new LanguageSwitcher(language))
                _sitecoreServiceProvider.GetSitecoreService().Delete(model);
        }

        #region IRepository implementation
        T IRepository<T>.Get(object id)
        {
            if (id is Guid)
                return this.Get((Guid) id);
            if (ID.IsID(id + ""))
                return this.Get(new ID(id + "").ToGuid());
            
            return this.Get(id + "");
        }

        void IRepository<T>.Delete(T target)
        {
            this.Delete(target);
        }

        void IRepository<T>.DeleteById(object id)
        {
            T item = ((IRepository<T>)this).Get(id);
            if (item != null)
                this.Delete(item);
        }

        System.Linq.IQueryable<T> IRepository<T>.Query()
        {
            var query = _searchContextProvider.GetProviderSearchContext()
                .GetQueryable<T>(new CultureExecutionContext(GetLanguage(_dbProvider).CultureInfo));
            return AddStandardQueries(query);
        }


        private IQueryable<T> AddStandardQueries(IQueryable<T> query)
        {
            //try to query the _templates field
            if (TemplateIdsProperty != null && typeof(IEnumerable<>).IsAssignableFrom(TemplateIdsProperty.PropertyInfo.PropertyType.GetGenericTypeDefinition())
                                                && TemplateIdsProperty.PropertyInfo.PropertyType.IsConstructedGenericType)
            {
                var collectionType = TemplateIdsProperty.PropertyInfo.PropertyType.GenericTypeArguments.First();
                var isID = typeof(ID).IsAssignableFrom(collectionType);
                var isGuid = typeof(Guid).IsAssignableFrom(collectionType);
                if (isID || isGuid)
                {
                    var sitecoreService = _sitecoreServiceProvider.GetSitecoreService();
                    var config = sitecoreService.GlassContext.GetTypeConfiguration<SitecoreTypeConfiguration>(typeof(T));

                    var pe = Expression.Parameter(typeof(T));
                    var me = Expression.Property(pe, TemplateIdsProperty.PropertyInfo);
                    var ce = isGuid ? Expression.Constant(config.TemplateId.ToGuid()) : Expression.Constant(config.TemplateId);
                    var call = Expression.Call(typeof(Enumerable), "Contains", new[] { collectionType }, me, ce);
                    var lambda = Expression.Lambda<Func<T, bool>>(call, pe);
                    query = query.Where(lambda);
                }
            }
            //try to add language filter
            if (LanguageProperty != null)
            {
                var isString = typeof(string).IsAssignableFrom(LanguageProperty.PropertyInfo.PropertyType);
                var isLang = typeof(Language).IsAssignableFrom(LanguageProperty.PropertyInfo.PropertyType);
                var isCult = typeof(CultureInfo).IsAssignableFrom(LanguageProperty.PropertyInfo.PropertyType);
                if (isString || isLang || isCult)
                {
                    var sitecoreService = _sitecoreServiceProvider.GetSitecoreService();
                    var config = sitecoreService.GlassContext.GetTypeConfiguration<SitecoreTypeConfiguration>(typeof(T));
                    var currentLang = GetLanguage(_dbProvider);

                    var pe = Expression.Parameter(typeof(T));
                    var me = Expression.Property(pe, LanguageProperty.PropertyInfo);
                    var ce = isString ? Expression.Constant(currentLang.Name) : isCult ? Expression.Constant(currentLang.CultureInfo) : Expression.Constant(currentLang);
                    var call = Expression.Equal(me, ce);
                    var lambda = Expression.Lambda<Func<T, bool>>(call, pe);
                    query = query.Where(lambda);
                }
            }
            return query;
        }

        T IRepository<T>.Save(T target)
        {
            target.Id = default(Guid);
            return this.SaveOrUpdate(target);
        }

        T IRepository<T>.Update(T target)
        {
            return this.SaveOrUpdate(target);
        }

        T IRepository<T>.SaveOrUpdate(T target)
        {
            return this.SaveOrUpdate(target);
        }

        void IRepository<T>.Evict(T target)
        {
            global::Sitecore.Caching.CacheManager.GetItemCache(this._dbProvider.GetDatabase()).RemoveItem(new ID(target.Id));
        }

        object IRepository.Get(object id)
        {
            if (id is Guid)
                return this.Get((Guid)id);
            if (ID.IsID(id + ""))
                return this.Get(new ID(id + "").ToGuid());

            return this.Get(id + "");
        }

        void IRepository.Delete(object target)
        {
            this.Delete(target as T);
        }

        object IRepository.Save(object target)
        {
            var t = target as T;
            if (t == null)
                return null;
            t.Id = default(Guid);
            return this.SaveOrUpdate(target as T);
        }

        object IRepository.Update(object target)
        {
            return this.SaveOrUpdate(target as T);
        }

        object IRepository.SaveOrUpdate(object target)
        {
            return this.SaveOrUpdate(target as T);
        }

        void IRepository.Evict(object target)
        {
            var t = target as T;
            if (t != null)
                global::Sitecore.Caching.CacheManager.GetItemCache(this._dbProvider.GetDatabase()).RemoveItem(new ID(t.Id));
        }
        #endregion

        
        
    }

}
