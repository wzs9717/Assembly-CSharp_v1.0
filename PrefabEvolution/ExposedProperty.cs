using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PrefabEvolution
{
	[Serializable]
	public class ExposedProperty : BaseExposedData
	{
		public class PropertyInvocationChain
		{
			public class InvokeInfo
			{
				public MemberInfo member;

				public int index = -1;

				public object tempTarget;

				public Type valueType;

				public object GetValue(object target)
				{
					tempTarget = target;
					return GetMemberValue(target, member, index);
				}

				public void SetValue(object target, object value)
				{
					tempTarget = target;
					setValue(target, member, value, index);
				}

				public void SetValue(object value)
				{
					setValue(tempTarget, member, value, index);
				}
			}

			public object root;

			public string path;

			public InvokeInfo[] members;

			public object value
			{
				get
				{
					object obj = root;
					GetValidFieldName(ref obj, path);
					for (int i = 0; i < members.Length; i++)
					{
						obj = members[i].GetValue(obj);
					}
					return obj;
				}
				set
				{
					object obj = root;
					GetValidFieldName(ref obj, path);
					for (int i = 0; i < members.Length - 1; i++)
					{
						obj = members[i].GetValue(obj);
					}
					members[members.Length - 1].SetValue(obj, value);
					for (int num = members.Length - 2; num >= 0; num--)
					{
						InvokeInfo invokeInfo = members[num];
						InvokeInfo invokeInfo2 = members[num + 1];
						if (invokeInfo.member.MemberType == MemberTypes.Property || invokeInfo.valueType.IsValueType || invokeInfo2.valueType.IsValueType)
						{
							invokeInfo.SetValue(invokeInfo2.tempTarget);
						}
					}
				}
			}

			public bool isValid => members != null;

			public PropertyInvocationChain(object root, string path)
			{
				this.root = root;
				this.path = path;
				GetInstance(root, path, out members);
			}

			internal static object GetInstance(object obj, string path, out InvokeInfo[] members)
			{
				path = path.Replace(".Array.data", string.Empty);
				string[] array = path.Split('.');
				string[] array2 = array;
				object obj2 = obj;
				members = new InvokeInfo[array2.Length];
				try
				{
					int num = 0;
					string[] array3 = array2;
					foreach (string text in array3)
					{
						members[num] = new InvokeInfo();
						if (text.Contains("["))
						{
							string[] array4 = text.Split('[', ']');
							int index = int.Parse(array4[1]);
							members[num].index = index;
							obj2 = getField(obj2, array4[0], out members[num].member, index);
						}
						else
						{
							obj2 = getField(obj2, text, out members[num].member);
						}
						PropertyInfo propertyInfo = members[num].member as PropertyInfo;
						if (members[num].member is FieldInfo fieldInfo)
						{
							members[num].valueType = fieldInfo.FieldType;
						}
						else if (propertyInfo != null)
						{
							members[num].valueType = propertyInfo.PropertyType;
						}
						num++;
					}
					return obj2;
				}
				catch (Exception exception)
				{
					members = null;
					Debug.LogException(exception);
					return null;
				}
			}

			private static object GetMemberValue(object target, MemberInfo member, int index = -1)
			{
				object obj = null;
				if (member is FieldInfo fieldInfo)
				{
					obj = fieldInfo.GetValue(target);
				}
				if (member is PropertyInfo propertyInfo)
				{
					obj = propertyInfo.GetValue(target, null);
				}
				return (obj == null) ? null : ((index != -1) ? (obj as IList)[index] : obj);
			}

			private static void setValue(object target, MemberInfo member, object value, int index = -1)
			{
				if (index != -1)
				{
					target = GetMemberValue(target, member);
					(target as IList)[index] = value;
					return;
				}
				if (member is FieldInfo fieldInfo)
				{
					fieldInfo.SetValue(target, value);
				}
				if (member is PropertyInfo propertyInfo)
				{
					propertyInfo.SetValue(target, value, null);
				}
			}

			public static string GetValidFieldName(ref object obj, string fieldName)
			{
				if (obj is Renderer && fieldName == "m_Materials")
				{
					return "sharedMaterials";
				}
				if (obj is MeshFilter && fieldName == "m_Mesh")
				{
					return "sharedMesh";
				}
				if (obj is GameObject && fieldName == "m_IsActive")
				{
					obj = new GameObjectWrapper(obj as GameObject);
				}
				return fieldName;
			}

			private static object getField(object obj, string field, out MemberInfo member, int index = -1)
			{
				member = null;
				try
				{
					BindingFlags bindingAttr = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
					if (obj == null)
					{
						return null;
					}
					field = GetValidFieldName(ref obj, field);
					Type type = obj.GetType();
					member = type.GetField(field, bindingAttr);
					if (member == null && field.StartsWith("m_"))
					{
						member = type.GetField(field.Remove(0, 2), bindingAttr);
					}
					member = type.GetProperty(field, bindingAttr);
					if (member == null && field.StartsWith("m_"))
					{
						member = type.GetProperty(field.Remove(0, 2), bindingAttr);
					}
					if (member == null)
					{
						member = type.GetMembers(bindingAttr).First((MemberInfo m) => m.Name.ToUpper() == field.ToUpper());
					}
					if (member != null)
					{
						return GetMemberValue(obj, member, index);
					}
					member = null;
					return null;
				}
				catch (Exception)
				{
					member = null;
					return null;
				}
			}
		}

		public UnityEngine.Object Target;

		public string PropertyPath;

		private PropertyInvocationChain _invocationChain;

		private PropertyInvocationChain invocationChain
		{
			get
			{
				if (_invocationChain == null)
				{
					_invocationChain = new PropertyInvocationChain(Target, PropertyPath);
					try
					{
						Value = Value;
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						_invocationChain.members = null;
					}
				}
				return _invocationChain;
			}
		}

		public bool IsValid => invocationChain.isValid;

		public object Value
		{
			get
			{
				if (!IsValid)
				{
					Debug.LogWarning(string.Concat("Trying to get value from invalid prefab property. Target:", Target, " Property path:", PropertyPath));
					return null;
				}
				return invocationChain.value;
			}
			set
			{
				if (!IsValid)
				{
					Debug.LogWarning(string.Concat("Trying to set value to invalid prefab property. [Target:", Target, ", Property path:", PropertyPath, "]"));
				}
				else
				{
					invocationChain.value = value;
				}
			}
		}

		public override BaseExposedData Clone()
		{
			ExposedProperty exposedProperty = (ExposedProperty)base.Clone();
			exposedProperty.Target = Target;
			exposedProperty.PropertyPath = PropertyPath;
			return exposedProperty;
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			_invocationChain = null;
		}
	}
}
