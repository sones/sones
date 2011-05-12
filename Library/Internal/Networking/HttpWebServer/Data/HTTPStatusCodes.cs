/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.HTTP
{

    /// <summary>
    /// All HTTP Status Code needed for WebDAV
    /// Additional standard HTTP Codes can be found here: http://tools.ietf.org/html/rfc2616#section-10
    /// </summary>
    public enum HTTPStatusCodes : int
    {
        // Summary:
        // Could not recognize the HTTPStatusCodes
        Unknown = 0,

        // Summary:
        //     Equivalent to HTTP status 100. HTTPStatusCodes.Continue indicates
        //     that the client can continue with its request.
        Continue = 100,
        //
        // Summary:
        //     Equivalent to HTTP status 101. HTTPStatusCodes.SwitchingProtocols
        //     indicates that the protocol version or protocol is being changed.
        SwitchingProtocols = 101,
        //
        // Summary:
        //     Equivalent to HTTP status 200. HTTPStatusCodes.OK indicates that
        //     the request succeeded and that the requested information is in the response.
        //     This is the most common status code to receive.
        OK = 200,
        //
        // Summary:
        //     Equivalent to HTTP status 201. HTTPStatusCodes.Created indicates
        //     that the request resulted in a new resource created before the response was
        //     sent.
        Created = 201,
        //
        // Summary:
        //     Equivalent to HTTP status 202. HTTPStatusCodes.Accepted indicates
        //     that the request has been accepted for further processing.
        Accepted = 202,
        //
        // Summary:
        //     Equivalent to HTTP status 203. HTTPStatusCodes.NonAuthoritativeInformation
        //     indicates that the returned metainformation is from a cached copy instead
        //     of the origin server and therefore may be incorrect.
        NonAuthoritativeInformation = 203,
        //
        // Summary:
        //     Equivalent to HTTP status 204. HTTPStatusCodes.NoContent indicates
        //     that the request has been successfully processed and that the response is
        //     intentionally blank.
        NoContent = 204,
        //
        // Summary:
        //     Equivalent to HTTP status 205. HTTPStatusCodes.ResetContent indicates
        //     that the client should reset (not reload) the current resource.
        ResetContent = 205,
        //
        // Summary:
        //     Equivalent to HTTP status 206. HTTPStatusCodes.PartialContent indicates
        //     that the response is a partial response as requested by a GET request that
        //     includes a byte range.
        PartialContent = 206,
        //
        // Summary:
        //     Equivalent to HTTP status 207. HTTPStatusCodes.MultiStatus indicates
        //     that the response is a MultiStatus response as requested by a WebDAV PROPFIND request that
        //     contains the content of a requested destination.
        MultiStatus = 207,
        //
        // Summary:
        //     Equivalent to HTTP status 300. HTTPStatusCodes.MultipleChoices
        //     indicates that the requested information has multiple representations. The
        //     default action is to treat this status as a redirect and follow the contents
        //     of the Location header associated with this response.
        MultipleChoices = 300,
        //
        // Summary:
        //     Equivalent to HTTP status 300. HTTPStatusCodes.Ambiguous indicates
        //     that the requested information has multiple representations. The default
        //     action is to treat this status as a redirect and follow the contents of the
        //     Location header associated with this response.
        Ambiguous = 300,
        //
        // Summary:
        //     Equivalent to HTTP status 301. HTTPStatusCodes.MovedPermanently
        //     indicates that the requested information has been moved to the URI specified
        //     in the Location header. The default action when this status is received is
        //     to follow the Location header associated with the response.
        MovedPermanently = 301,
        //
        // Summary:
        //     Equivalent to HTTP status 301. HTTPStatusCodes.Moved indicates
        //     that the requested information has been moved to the URI specified in the
        //     Location header. The default action when this status is received is to follow
        //     the Location header associated with the response. When the original request
        //     method was POST, the redirected request will use the GET method.
        Moved = 301,
        //
        // Summary:
        //     Equivalent to HTTP status 302. HTTPStatusCodes.Found indicates
        //     that the requested information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the
        //     Location header associated with the response. When the original request method
        //     was POST, the redirected request will use the GET method.
        Found = 302,
        //
        // Summary:
        //     Equivalent to HTTP status 302. HTTPStatusCodes.Redirect indicates
        //     that the requested information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the
        //     Location header associated with the response. When the original request method
        //     was POST, the redirected request will use the GET method.
        Redirect = 302,
        //
        // Summary:
        //     Equivalent to HTTP status 303. HTTPStatusCodes.SeeOther automatically
        //     redirects the client to the URI specified in the Location header as the result
        //     of a POST. The request to the resource specified by the Location header will
        //     be made with a GET.
        SeeOther = 303,
        //
        // Summary:
        //     Equivalent to HTTP status 303. HTTPStatusCodes.RedirectMethod automatically
        //     redirects the client to the URI specified in the Location header as the result
        //     of a POST. The request to the resource specified by the Location header will
        //     be made with a GET.
        RedirectMethod = 303,
        //
        // Summary:
        //     Equivalent to HTTP status 304. HTTPStatusCodes.NotModified indicates
        //     that the client's cached copy is up to date. The contents of the resource
        //     are not transferred.
        NotModified = 304,
        //
        // Summary:
        //     Equivalent to HTTP status 305. HTTPStatusCodes.UseProxy indicates
        //     that the request should use the proxy server at the URI specified in the
        //     Location header.
        UseProxy = 305,
        //
        // Summary:
        //     Equivalent to HTTP status 306. HTTPStatusCodes.Unused is a proposed
        //     extension to the HTTP/1.1 specification that is not fully specified.
        Unused = 306,
        //
        // Summary:
        //     Equivalent to HTTP status 307. HTTPStatusCodes.TemporaryRedirect
        //     indicates that the request information is located at the URI specified in
        //     the Location header. The default action when this status is received is to
        //     follow the Location header associated with the response. When the original
        //     request method was POST, the redirected request will also use the POST method.
        TemporaryRedirect = 307,
        //
        // Summary:
        //     Equivalent to HTTP status 307. HTTPStatusCodes.RedirectKeepVerb
        //     indicates that the request information is located at the URI specified in
        //     the Location header. The default action when this status is received is to
        //     follow the Location header associated with the response. When the original
        //     request method was POST, the redirected request will also use the POST method.
        RedirectKeepVerb = 307,
        //
        // Summary:
        //     Equivalent to HTTP status 400. HTTPStatusCodes.BadRequest indicates
        //     that the request could not be understood by the server. HTTPStatusCodes.BadRequest
        //     is sent when no other error is applicable, or if the exact error is unknown
        //     or does not have its own error code.
        BadRequest = 400,
        //
        // Summary:
        //     Equivalent to HTTP status 401. HTTPStatusCodes.Unauthorized indicates
        //     that the requested resource requires authentication. The WWW-Authenticate
        //     header contains the details of how to perform the authentication.
        Unauthorized = 401,
        //
        // Summary:
        //     Equivalent to HTTP status 402. HTTPStatusCodes.PaymentRequired
        //     is reserved for future use.
        PaymentRequired = 402,
        //
        // Summary:
        //     Equivalent to HTTP status 403. HTTPStatusCodes.Forbidden indicates
        //     that the server refuses to fulfill the request.
        Forbidden = 403,
        //
        // Summary:
        //     Equivalent to HTTP status 404. HTTPStatusCodes.NotFound indicates
        //     that the requested resource does not exist on the server.
        NotFound = 404,
        //
        // Summary:
        //     Equivalent to HTTP status 405. HTTPStatusCodes.MethodNotAllowed
        //     indicates that the request method (POST or GET) is not allowed on the requested
        //     resource.
        MethodNotAllowed = 405,
        //
        // Summary:
        //     Equivalent to HTTP status 406. HTTPStatusCodes.NotAcceptable indicates
        //     that the client has indicated with Accept headers that it will not accept
        //     any of the available representations of the resource.
        NotAcceptable = 406,
        //
        // Summary:
        //     Equivalent to HTTP status 407. HTTPStatusCodes.ProxyAuthenticationRequired
        //     indicates that the requested proxy requires authentication. The Proxy-authenticate
        //     header contains the details of how to perform the authentication.
        ProxyAuthenticationRequired = 407,
        //
        // Summary:
        //     Equivalent to HTTP status 408. HTTPStatusCodes.RequestTimeout indicates
        //     that the client did not send a request within the time the server was expecting
        //     the request.
        RequestTimeout = 408,
        //
        // Summary:
        //     Equivalent to HTTP status 409. HTTPStatusCodes.Conflict indicates
        //     that the request could not be carried out because of a conflict on the server.
        Conflict = 409,
        //
        // Summary:
        //     Equivalent to HTTP status 410. HTTPStatusCodes.Gone indicates that
        //     the requested resource is no longer available.
        Gone = 410,
        //
        // Summary:
        //     Equivalent to HTTP status 411. HTTPStatusCodes.LengthRequired indicates
        //     that the required Content-length header is missing.
        LengthRequired = 411,
        //
        // Summary:
        //     Equivalent to HTTP status 412. HTTPStatusCodes.PreconditionFailed
        //     indicates that a condition set for this request failed, and the request cannot
        //     be carried out. Conditions are set with conditional request headers like
        //     If-Match, If-None-Match, or If-Unmodified-Since.
        PreconditionFailed = 412,
        //
        // Summary:
        //     Equivalent to HTTP status 413. HTTPStatusCodes.RequestEntityTooLarge
        //     indicates that the request is too large for the server to process.
        RequestEntityTooLarge = 413,
        //
        // Summary:
        //     Equivalent to HTTP status 414. HTTPStatusCodes.RequestUriTooLong
        //     indicates that the URI is too long.
        RequestUriTooLong = 414,
        //
        // Summary:
        //     Equivalent to HTTP status 415. HTTPStatusCodes.UnsupportedMediaType
        //     indicates that the request is an unsupported type.
        UnsupportedMediaType = 415,
        //
        // Summary:
        //     Equivalent to HTTP status 416. HTTPStatusCodes.RequestedRangeNotSatisfiable
        //     indicates that the range of data requested from the resource cannot be returned,
        //     either because the beginning of the range is before the beginning of the
        //     resource, or the end of the range is after the end of the resource.
        RequestedRangeNotSatisfiable = 416,
        //
        // Summary:
        //     Equivalent to HTTP status 417. HTTPStatusCodes.ExpectationFailed
        //     indicates that an expectation given in an Expect header could not be met
        //     by the server.
        ExpectationFailed = 417,
        //
        // Summary:
        //     Equivalent to HTTP status 422. HTTPStatusCodes.UnprocessableEntity
        //     indicates an UnprocessableEntity.
        UnprocessableEntity = 422,
        //
        // Summary:
        //     Equivalent to HTTP status 423. HTTPStatusCodes.Locked
        //     indicates a successful lock request for a WebDAV destination.
        Locked = 423,
        //
        // Summary:
        //     Equivalent to HTTP status 424. HTTPStatusCodes.FailedDependency
        //     indicates a FailedDependency of a WebDAV request.
        FailedDependency = 424,
        //
        // Summary:
        //     Equivalent to HTTP status 500. HTTPStatusCodes.InternalServerError
        //     indicates that a generic error has occurred on the server.
        InternalServerError = 500,
        //
        // Summary:
        //     Equivalent to HTTP status 501. HTTPStatusCodes.NotImplemented indicates
        //     that the server does not support the requested function.
        NotImplemented = 501,
        //
        // Summary:
        //     Equivalent to HTTP status 502. HTTPStatusCodes.BadGateway indicates
        //     that an intermediate proxy server received a bad response from another proxy
        //     or the origin server.
        BadGateway = 502,
        //
        // Summary:
        //     Equivalent to HTTP status 503. HTTPStatusCodes.ServiceUnavailable
        //     indicates that the server is temporarily unavailable, usually due to high
        //     load or maintenance.
        ServiceUnavailable = 503,
        //
        // Summary:
        //     Equivalent to HTTP status 504. HTTPStatusCodes.GatewayTimeout indicates
        //     that an intermediate proxy server timed out while waiting for a response
        //     from another proxy or the origin server.
        GatewayTimeout = 504,
        //
        // Summary:
        //     Equivalent to HTTP status 505. HTTPStatusCodes.HttpVersionNotSupported
        //     indicates that the requested HTTP version is not supported by the server.
        HttpVersionNotSupported = 505,
        //
        // Summary:
        //     Equivalent to HTTP status 507. HTTPStatusCodes.InsufficientStorage
        //     indicates a InsufficientStorage of a WebDAV upload request.
        InsufficientStorage = 507

        /*
        OK = 200,
        Created = 201,
        MultiStatus = 207,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        PreconditionFailed = 412,
        RequestURITooLong = 414,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        NotImplemented = 501,
        InsufficientStorage = 507
        * */
    }

}
