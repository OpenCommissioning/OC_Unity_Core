using System.Collections.Generic;

namespace OC.MaterialFlow
{
    public static class PayloadUtils
    {
        public static bool IsTypeValid(PayloadBase payloadBase, CollisionFilter filter)
        {
            return payloadBase switch
            {
                PayloadStorage => filter.HasFlag(CollisionFilter.Storage),
                StaticCollider => filter.HasFlag(CollisionFilter.Static),
                Payload payload => ((int)filter & 2.Pow((int)payload.Category)) != 0,
                _ => false
            };
        }
        
        public static bool IsGroupValid(int groupId, int requiredGroupId)
        {
            if (requiredGroupId == 0) return true;
            return requiredGroupId == groupId;
        }

        public static PayloadStorage GetLastPayloadStorage(this IList<PayloadBase> payloads)
        {
            for (var i = payloads.Count - 1; i >= 0; i--)
            {
                if (payloads[i] is not PayloadStorage e) continue;
                return e;
            }
            return null;
        }

        public static Payload GetLastByType(this IList<PayloadBase> payloads, Payload.PayloadCategory type)
        {
            for (var i = payloads.Count - 1; i >= 0; i--)
            {
                if (payloads[i] is not Payload e) continue;
                if (e.Category == type)
                {
                    return e;
                }
            }
            return null;
        }
    }
}