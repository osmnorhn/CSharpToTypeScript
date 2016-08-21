using Generic.Utils.Extensions;
using Generic.Utils.Reflection;
using System;
using System.Reflection;
using System.Text;
using TsModelGenerator;

namespace CSharpToTypeScript
{
    class CSharpToTypeScript
    {
        public interface ITypeScriptCodeWriter<in TSource>
        {
            void Write(StringBuilder sb, TSource source);
        }

        public sealed class PropertyWriter : ITypeScriptCodeWriter<PropertyInfo>
        {
            public void Write(StringBuilder sb, PropertyInfo pi)
            {
                sb.Append("    ");
                sb.Append(pi.Name)
                    .Append("?: ") //Interface olduğu içim optional
                    .Append(pi.ToJsType());
                if (ReflectionExtensions.IsEnumerable(pi))
                    sb.Append("[]");
                sb.Append(";");
            }
        }

        public sealed class InterfaceWtiter : ITypeScriptCodeWriter<Type>
        {
            public void Write(StringBuilder sb, Type source)
            {
                PropertyInfo[] pis = source.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (pis.IsEmptyList()) return;

                sb.Append("export interface ")
                    .Append(source.Name)
                    .Append(" { ");

                PropertyWriter pw = new PropertyWriter();
                foreach (PropertyInfo pi in pis)
                {
                    sb.AppendLine();
                    pw.Write(sb, pi);
                }

                sb.AppendLine().Append("}");
            }
        }

        public sealed class ModuleWriter : ITypeScriptCodeWriter<Assembly>
        {
            public void Write(StringBuilder text, Assembly asm)
            {
                InterfaceWtiter iw = new InterfaceWtiter();
                foreach (Type type in asm.GetTypes())
                {
                    iw.Write(text, type);
                    text.AppendLine().AppendLine();
                }
            }
        }
    }
}
