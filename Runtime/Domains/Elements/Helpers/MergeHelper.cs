using System;

namespace HeadWindCSS.Domains.Elements.Helpers
{
    using ClassValue = String;
    
    public static class MergeHelper
    {
        static ClassValue[] Clsx(ClassValue[] inputs)
        {
            // TODO calculate which classes should be discard
            return new ClassValue[] { };
        }

        static ClassValue[] MergeClass(ClassValue[] inputs)
        {
            return Clsx(inputs);
        }
    }
}