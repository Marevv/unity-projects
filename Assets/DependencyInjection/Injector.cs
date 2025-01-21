using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;


namespace DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute { }

    public interface IDependencyProvider { }

    [DefaultExecutionOrder(-1000)]
    public class Injector : Singleton<Injector>
    {
        const BindingFlags k_bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        readonly Dictionary<Type, object> registry = new Dictionary<Type, object>();

        protected override void Awake()
        {
            base.Awake();



            var providers = findMonoBehaviours().OfType<IDependencyProvider>();
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }

            var injectables = findMonoBehaviours().Where(IsInjectable);
            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }

        }

        void Inject(object instance)
        {
            FieldInjection(instance);
            MethodInjection(instance);
            PropertyInjection(instance);
        }

        void FieldInjection(object instance)
        {
            var type = instance.GetType();
            var injectableFields = type.GetFields(k_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields)
            {
                var fieldType = injectableField.FieldType;
                var resolvedInstance = Resolve(fieldType);
                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to Inject {fieldType.Name} into {type.Name}");
                }

                injectableField.SetValue(instance, resolvedInstance);

                Debug.Log($"Field Injected {fieldType.Name} into {type.Name}");
            }
        }

        void MethodInjection(object instance)
        {
            var type = instance.GetType();
            var injectableMethods = type.GetMethods(k_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods)
            {
                var requiredParameters = injectableMethod.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .ToArray();

                var resolvedInstances = requiredParameters.Select(Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstances => resolvedInstances == null))
                {
                    throw new Exception($"Failed to inject {type.Name}.{injectableMethod.Name}");
                }


                injectableMethod.Invoke(instance, resolvedInstances);
                Debug.Log($"Method Injected {type.Name}.{injectableMethod.Name}");

            }
        }

        void PropertyInjection(object instance)
        {
            var type = instance.GetType();
            var injectableProperties = type.GetProperties(k_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableProperty in injectableProperties)
            {
                var propertyType = injectableProperty.PropertyType;
                var resolvedInstance = Resolve(propertyType);

                if (resolvedInstance == null)
                {
                    throw new Exception($"Failed to Inject {propertyType.Name} into {type.Name}");
                }

                injectableProperty.SetValue(instance, resolvedInstance);

                Debug.Log($"Property Injected {propertyType.Name} into {type.Name}");
            }
        }

        object Resolve(Type type)
        {
            registry.TryGetValue(type, out var resolvedInstance);
            return resolvedInstance;
        }

        static bool IsInjectable(MonoBehaviour obj)
        {
            var members = obj.GetType().GetMembers(k_bindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(k_bindingFlags);
            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

                var returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);

                if (providedInstance != null)
                {
                    registry.Add(returnType, providedInstance);
                    Debug.Log($"Resitered {returnType.Name} from {provider.GetType().Name}");
                }
                else
                {
                    throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
                }

            }
        }

        static MonoBehaviour[] findMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }
    }
}