using UnityEngine;

namespace OC.Communication
{
    public static class LinkExtension
    {
        /// <summary>
        /// Retrieves the name of the GameObject associated with the given <see cref="Link"/>. 
        /// If the name is not a valid variable name according to <see cref="ClientVariableExtension"/>, 
        /// it will be corrected on the GameObject and a warning will be logged in the Unity Editor.
        /// </summary>
        /// <param name="link">The <see cref="Link"/> whose GameObject name is being retrieved and validated.</param>
        /// <returns>The (original) name of the GameObject. Note that the GameObject’s name is modified in-editor if it was invalid.</returns>
        public static string GetName(this Link link)
        {
            return link.Component.gameObject.name;
        }
        
        /// <summary>
        /// Constructs the full hierarchical name for the GameObject associated with the given <see cref="Link"/>. 
        /// Starting from the GameObject’s own name, it traverses upward through its parent transforms. 
        /// For each parent that has a <see cref="Hierarchy"/> component marked as a sampler (<see cref="Hierarchy.IsNameSampler"/>),
        /// its <see cref="Hierarchy.Name"/> is prepended (followed by an underscore) to the current name. 
        /// The process stops when there are no more sampler hierarchies in the chain.
        /// </summary>
        /// <param name="link">The <see cref="Link"/> whose GameObject full name is being assembled.</param>
        /// <returns>
        /// A string representing the combined sampler names and the original GameObject name, 
        /// separated by underscores (e.g. "RootSampler_SubSampler_ObjectName").
        /// </returns>
        public static string GetHierarchyName(this Link link)
        {
            var name = link.GetName();
            var parent = link.Parent != null ?  link.Parent.transform : link.Component.transform.parent; 
            
            while (parent != null)
            {
                if (parent.TryGetComponent<Hierarchy>(out var hierarchy))
                {
                    if (hierarchy.IsNameSampler)
                    {
                        name = hierarchy.Name + "_" + name;
                        parent = hierarchy.GetParent();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    parent = parent.parent;
                }
            }

            return name;
        }
        
        /// <summary>
        /// Builds a dot-separated path representing the link’s position in the client/hierarchy structure.
        /// Starts with the link’s own name (validated via <see cref="GetName"/>), then walks up through
        /// parent transforms. If a <see cref="Client"/> is encountered, its <see cref="Client.RootName"/>
        /// is prepended and the traversal ends. Otherwise, for each <see cref="Hierarchy"/> parent, its
        /// <see cref="Hierarchy.Name"/> is prepended using “.” or “_” if <see cref="Hierarchy.IsNameSampler"/>
        /// is true. If <paramref name="original"/> is false and the link has a <see cref="Link.Parent"/>,
        /// the traversal uses that chain; otherwise it uses the GameObject’s raw transform hierarchy.
        /// </summary>
        /// <param name="link">The <see cref="Link"/> whose hierarchy path is being constructed.</param>
        /// <param name="original">
        /// If false and <paramref name="link"/> has a non-null <see cref="Link.Parent"/>, use the parent link’s
        /// transform chain; otherwise use the GameObject’s direct transform parents.
        /// </param>
        /// <returns>
        /// A string combining client root and hierarchy names, separated by “.” (or “_” for samplers),
        /// e.g. “RootClient.ParentName.ChildName” or “Sampler1_Sampler2_ObjectName”.
        /// </returns>
        public static string GetHierarchyPath(this Link link, bool original = false)
        {
            var path = link.GetName();
            
            Transform parent;

            if (!original && link.Parent != null)
            {
                parent = link.Parent.transform;
            }
            else
            {
                parent = link.Component.transform.parent;
            }

            while (parent != null)
            {
                if (parent.TryGetComponent<Client>(out var client))
                {
                    return client.RootName + "." + path;
                }
                
                if (parent.TryGetComponent<Hierarchy>(out var hierarchy))
                {
                    path = hierarchy.IsNameSampler ? hierarchy.Name + "_" + path : hierarchy.Name + "." + path;
                    parent = original ? hierarchy.transform.parent : hierarchy.GetParent();
                    continue;
                }
                
                parent = parent.parent;
            }

            return path;
        }

        public static bool IsPathOriginal(this Link link)
        {
            if (link.Parent != null) return false;
            var parent = link.Component.transform.parent; 
            while (parent != null)
            {
                if (parent.TryGetComponent<Client>(out _))
                {
                    return true;
                }

                if (parent.TryGetComponent<Hierarchy>(out var hierarchy))
                {
                    if (hierarchy.Parent != null)
                    {
                        return false;
                    }
                    
                    parent = hierarchy.transform.parent;
                }
                else
                {
                    parent = parent.parent;
                }
            }

            return true;
        }
        
        public static Client GetClient(this Link link)
        {
            var parent = link.Parent != null ?  link.Parent.transform : link.Component.transform.parent;
            
            while (parent != null)
            {
                if (parent.TryGetComponent<Client>(out var client))
                {
                    return client;
                }
                
                parent = parent.parent;
            }

            return null;
        }
    }
}
