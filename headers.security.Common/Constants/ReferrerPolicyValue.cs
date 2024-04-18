namespace headers.security.Common.Constants;

public static class ReferrerPolicyValue
{
                                                                                         // same-origin     	same-origin, down	cross-origin    	cross-origin, down
    public const string NoReferrer                  = "no-referrer";                     // NONE            	NONE	            NONE	            NONE
    public const string NoReferrerWhenDowngrade     = "no-referrer-when-downgrade";      // origin/path/query	NONE	            origin/path/query	NONE
    public const string Origin                      = "origin";                          // origin	            origin	            origin	            origin
    public const string OriginWhenCrossOrigin       = "origin-when-cross-origin";        // origin/path/query	origin	            origin	            origin
    public const string SameOrigin                  = "same-origin";                     // origin/path/query	origin/path/query	NONE	            NONE
    public const string StrictOrigin                = "strict-origin";                   // origin	            NONE	            origin	            NONE
    public const string StrictOriginWhenCrossOrigin = "strict-origin-when-cross-origin"; // origin/path/query	NONE	            origin	            NONE               <-- Default
    public const string UnsafeUrl                   = "unsafe-url";                      // origin/path/query	origin/path/query	origin/path/query	origin/path/query

}