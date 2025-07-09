using System;
using System.Collections;
using UnityEngine;

namespace OC.Tests
{
    public class Wait 
    {
        public static IEnumerator Until(Func<bool> condition, float timeout = 30f)
        {
            var timePassed = 0f;
            while (!condition() && timePassed < timeout) 
            {
                yield return new WaitForEndOfFrame();
                timePassed += Time.deltaTime;
            }
            if (timePassed >= timeout) 
            {
                throw new TimeoutException("Condition was not fulfilled for " + timeout + " seconds.");
            }
        }
    }
}
