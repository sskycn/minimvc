using System;
using System.Collections.Generic;
using System.Text;
using Cvv.WebUtility.Mvc;
using Cvv.WebUtility.Components;

namespace Cvv.WebUtility.Mvc.Provider
{
    public interface ISecurityProvider
    {
        /// <summary>
        /// Redirect to signin page
        /// </summary>
        /// <param name="returnUrl"></param>
        void SignIn(string returnUrl);

        /// <summary>
        /// Assign user id and right to session
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="right"></param>
        void SignIn(long userId, long[] right);

        /// <summary>
        /// Assign user id and right to session
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="right"></param>
        void SignIn(long userId, long right);

        /// <summary>
        /// Authorize
        /// </summary>
        /// <param name="code"></param>
        /// <returns>1 success, -1 signature error, -2 timeout, -3 data error, -500 unknow error</returns>
        int Authorize(string code);

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="returnUrl"></param>
        void Logout(string returnUrl);

        /// <summary>
        /// Logout
        /// </summary>
        void Logout();

        /// <summary>
        /// Calculate Right
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        long CalculateRight(long[] right);

        /// <summary>
        /// Check permission for View, use CHECK in view will invoke it
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionMethod"></param>
        /// <returns></returns>
        bool Check(string controllerName, string actionMethod);

        /// <summary>
        /// Exec binary and
        /// </summary>
        /// <param name="rightSrc"></param>
        /// <param name="rightDist"></param>
        /// <returns></returns>
        bool Check(long rightSrc, long rightDist);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightSrc"></param>
        /// <param name="rightDist1"></param>
        /// <param name="rightDist2"></param>
        /// <returns></returns>
        bool Check(long rightSrc, long rightDist1, long rightDist2);

        /// <summary>
        /// Check permission for Controller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionMethod"></param>
        /// <returns></returns>
        bool CheckPermission(string controllerName, string actionMethod);

        /// <summary>
        /// Remove permission cache by key
        /// </summary>
        /// <param name="key"></param>
        void RemoveCache(string key);

        /// <summary>
        /// Remove permission cache by userid
        /// </summary>
        /// <param name="userId"></param>
        void RemoveCache(long userId);

        /// <summary>
        /// Clear all permission cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// RSA Encrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        string Encrypt(string data, string publicKey);

        /// <summary>
        ///  RSA Decrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        string Decrypt(string data, string privateKey);
    }
}
