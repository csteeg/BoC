using System.Security.Permissions;
using System.Web;

namespace Microsoft.Web.DynamicData.Mvc {
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public enum HttpStatusCode {
        /// <summary>HTTP Status Code 200: OK</summary>
        OK = 200,
        /// <summary>HTTP Status Code 201: Created</summary>
        Created = 201,
        /// <summary>HTTP Status Code 202: Accepted</summary>
        Accepted = 202,
        /// <summary>HTTP Status Code 203: Non-authoritative information</summary>
        NonAuthoritativeInformation = 203,
        /// <summary>HTTP Status Code 204: No content</summary>
        NoContent = 204,
        /// <summary>HTTP Status Code 205: Reset content</summary>
        ResetContent = 205,
        /// <summary>HTTP Status Code 206: Partial content</summary>
        PartialContent = 206,
        /// <summary>HTTP Status Code 300: Multiple choices</summary>
        MultipleChoices = 300,
        /// <summary>HTTP Status Code 301: Redirect (moved permanently)</summary>
        MovedPermanently = 301,
        /// <summary>HTTP Status Code 302: Redirect (moved temporarily)</summary>
        MovedTemporarily = 302,
        /// <summary>HTTP Status Code 303: See other</summary>
        SeeOther = 303,
        /// <summary>HTTP Status Code 304: Not modified</summary>
        NotModified = 304,
        /// <summary>HTTP Status Code 305: Use proxy</summary>
        UseProxy = 305,
        /// <summary>HTTP Status Code 400: Bad request</summary>
        BadRequest = 400,
        /// <summary>HTTP Status Code 401: Unauthorized</summary>
        Unauthorized = 401,
        /// <summary>HTTP Status Code 402: Payment required</summary>
        PaymentRequired = 402,
        /// <summary>HTTP Status Code 403: Forbidden</summary>
        Forbidden = 403,
        /// <summary>HTTP Status Code 404: Not found</summary>
        NotFound = 404,
        /// <summary>HTTP Status Code 405: Method not allowed</summary>
        MethodNotAllowed = 405,
        /// <summary>HTTP Status Code 406: Not acceptable</summary>
        NotAcceptable = 406,
        /// <summary>HTTP Status Code 407: Proxy authorization required</summary>
        ProxyAuthenticationRequired = 407,
        /// <summary>HTTP Status Code 408: Request timeout</summary>
        RequestTimeout = 408,
        /// <summary>HTTP Status Code 409: Conflict</summary>
        Conflict = 409,
        /// <summary>HTTP Status Code 410: Gone</summary>
        Gone = 410,
        /// <summary>HTTP Status Code 411: Length required</summary>
        LengthRequired = 411,
        /// <summary>HTTP Status Code 412: Precondition failed</summary>
        PreconditionFailed = 412,
        /// <summary>HTTP Status Code 413: Request entity too large</summary>
        RequestEntityTooLarge = 413,
        /// <summary>HTTP Status Code 414: Request URI too long</summary>
        RequestUriTooLong = 414,
        /// <summary>HTTP Status Code 415: Unsupported media type</summary>
        UnsupportedMediaType = 415,
        /// <summary>HTTP Status Code 416: Request range not satisfiable</summary>
        RequestRangeNotSatisfiable = 416,
        /// <summary>HTTP Status Code 417: Expectation failed</summary>
        ExpectationFailed = 417,
        /// <summary>HTTP Status Code 500: Internal server error</summary>
        InternalServerError = 500,
        /// <summary>HTTP Status Code 501: Not implemented</summary>
        NotImplemented = 501,
        /// <summary>HTTP Status Code 502: Bad gateway</summary>
        BadGateway = 502,
        /// <summary>HTTP Status Code 503: Service unavailable</summary>
        ServiceUnavailable = 503,
        /// <summary>HTTP Status Code 504: Gateway timeout</summary>
        GatewayTimeout = 504,
        /// <summary>HTTP Status Code 505: HTTP version not supported</summary>
        HttpVersionNotSupported = 505
    }
}