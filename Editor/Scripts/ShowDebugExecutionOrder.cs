using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class ShowDebugExecutionOrder : EditorWindow
    {
        class AssemblyInfo
        {
            public Type Assembly;
            public DefaultExecutionOrder Attribute;
        }

        private List<AssemblyInfo> _defaultExecutionOrders;

        [MenuItem("Open Commissioning/Utility/Show Default Execution Order")]
        public static void ShowExample()
        {
            ShowDebugExecutionOrder wnd = GetWindow<ShowDebugExecutionOrder>();
            wnd.titleContent = new GUIContent("ShowDebugExecutionOrder");
        }

        public ShowDebugExecutionOrder()
        {
            GetDefaultExecutionOrders();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var scrollView = new ScrollView();

            foreach (var order in _defaultExecutionOrders)
            {
                var layout = new VisualElement();
                layout.style.flexDirection = FlexDirection.Row;

                var assembly = new Label(order.Assembly.FullName);
                layout.Add(assembly);

                var attribute = new Label(order.Attribute.order.ToString());
                layout.Add(attribute);

                scrollView.Add(layout);
            }

            root.Add(scrollView);
        }

        public void GetDefaultExecutionOrders()
        {

            _defaultExecutionOrders = new List<AssemblyInfo>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
#if (NET_4_6 || NET_STANDARD_2_0)
                if (assembly.IsDynamic)
                    continue;
#endif

                try
                {
                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        DefaultExecutionOrder attribute = (DefaultExecutionOrder)type.GetCustomAttribute(typeof(DefaultExecutionOrder), false);
                        if (attribute == null) continue;
                        _defaultExecutionOrders.Add(new AssemblyInfo() { Assembly = type, Attribute = attribute });
                    }
                }
                catch (NotSupportedException) { }
                catch (System.IO.FileNotFoundException) { }
                catch (ReflectionTypeLoadException) { }
                catch (Exception e)
                {
                    Debug.LogError("Couldn't search assembly for [DefaultExecutionOrder] attributes: " + assembly.FullName + "\n" + e);
                }

            }

            _defaultExecutionOrders = _defaultExecutionOrders.OrderBy(o => o.Attribute.order).ToList();
        }
    } 
}