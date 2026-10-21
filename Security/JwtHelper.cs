using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace Student_Att.Security
{
    // Creates and reads JSON Web Tokens (JWT).
    // A JWT has 3 parts separated by dots:   header.payload.signature
    //   header    = which algorithm signed it (HS256)
    //   payload   = the data (user id, name, role, expiry)
    //   signature = HMAC-SHA256(header + "." + payload, secret key)
    // Anyone can READ a JWT, but nobody can CHANGE it without the secret key,
    // because the signature would no longer match.
    public static class JwtHelper
    {
        public static string CreateToken(TokenInfo info)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<string, object> header = new Dictionary<string, object>();
            header.Add("alg", "HS256");
            header.Add("typ", "JWT");

            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("sub", info.UserId);                      // subject = user id
            payload.Add("name", info.FullName);
            payload.Add("role", info.Role);
            payload.Add("jti", info.TokenId.ToString());          // unique id of this token
            payload.Add("iat", ToUnixTime(DateTime.UtcNow));      // issued at
            payload.Add("exp", ToUnixTime(info.ExpiresAtUtc));    // expires at

            string headerPart = Base64UrlEncode(Encoding.UTF8.GetBytes(serializer.Serialize(header)));
            string payloadPart = Base64UrlEncode(Encoding.UTF8.GetBytes(serializer.Serialize(payload)));
            string signaturePart = Base64UrlEncode(Sign(headerPart + "." + payloadPart));

            return headerPart + "." + payloadPart + "." + signaturePart;
        }

        // Returns null if the token is broken, changed, or expired
        public static TokenInfo ReadToken(string token)
        {
            try
            {
                string[] parts = token.Split('.');
                if (parts.Length != 3)
                {
                    return null;
                }

                // 1. Check the signature. We always use HS256 with OUR key and never
                //    trust the "alg" written in the header (blocks the "alg: none" trick).
                byte[] expectedSignature = Sign(parts[0] + "." + parts[1]);
                byte[] actualSignature = Base64UrlDecode(parts[2]);
                if (!SecureCompare.AreEqual(expectedSignature, actualSignature))
                {
                    return null;
                }

                // 2. Read the payload
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
                Dictionary<string, object> payload = serializer.Deserialize<Dictionary<string, object>>(payloadJson);

                // 3. Check the expiry time
                long expiresAt = Convert.ToInt64(payload["exp"]);
                if (expiresAt <= ToUnixTime(DateTime.UtcNow))
                {
                    return null;
                }

                TokenInfo info = new TokenInfo();
                info.UserId = Convert.ToInt32(payload["sub"]);
                info.FullName = Convert.ToString(payload["name"]);
                info.Role = Convert.ToString(payload["role"]);
                info.TokenId = Guid.Parse(Convert.ToString(payload["jti"]));
                info.ExpiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expiresAt).UtcDateTime;
                return info;
            }
            catch (FormatException)
            {
                return null;   // bad Base64 or bad Guid
            }
            catch (ArgumentException)
            {
                return null;   // bad JSON
            }
            catch (KeyNotFoundException)
            {
                return null;   // a required value is missing
            }
            catch (InvalidCastException)
            {
                return null;   // a value has the wrong type
            }
            catch (OverflowException)
            {
                return null;   // a number is too big
            }
        }

        private static byte[] Sign(string data)
        {
            using (HMACSHA256 hmac = new HMACSHA256(GetSecretKey()))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        // The secret key lives in Secrets.config, never in the code
        private static byte[] GetSecretKey()
        {
            string secret = ConfigurationManager.AppSettings["JwtSecret"];

            if (string.IsNullOrEmpty(secret) || secret.Length < 32)
            {
                throw new InvalidOperationException(
                    "JwtSecret is missing or shorter than 32 characters. Check Secrets.config.");
            }

            return Encoding.UTF8.GetBytes(secret);
        }

        private static long ToUnixTime(DateTime utcTime)
        {
            return new DateTimeOffset(utcTime).ToUnixTimeSeconds();
        }

        // Base64Url = normal Base64 but safe for URLs and cookies:
        // '+' becomes '-', '/' becomes '_', and the '=' padding is removed
        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string text)
        {
            string base64 = text.Replace('-', '+').Replace('_', '/');

            // Put back the '=' padding so the length is a multiple of 4
            while (base64.Length % 4 != 0)
            {
                base64 = base64 + "=";
            }

            return Convert.FromBase64String(base64);
        }
    }
}
