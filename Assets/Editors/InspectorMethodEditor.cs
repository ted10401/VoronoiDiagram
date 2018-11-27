#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(UnityEngine.Object), true)]
public class InspectorMethodEditor : Editor
{
    private Dictionary<MethodInfo, object[]> m_parameterDatas;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var methodInfos = target.GetType()
                                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if(m_parameterDatas == null)
        {
            m_parameterDatas = new Dictionary<MethodInfo, object[]>();
        }

        foreach (MethodInfo methodInfo in methodInfos)
        {
            OnDrawInspectorButtons(methodInfo);
            OnDrawInspectorElements<int>(methodInfo, value => EditorGUILayout.IntField(value));
            OnDrawInspectorElements<float>(methodInfo, value => EditorGUILayout.FloatField(value));
            OnDrawInspectorElements<long>(methodInfo, value => EditorGUILayout.LongField(value));
            OnDrawInspectorElements<Rect>(methodInfo, value => EditorGUILayout.RectField(value));
            OnDrawInspectorElements<string>(methodInfo, value => EditorGUILayout.TextField(value));
            OnDrawInspectorElements<Color>(methodInfo, value => EditorGUILayout.ColorField(value));
            OnDrawInspectorElements<AnimationCurve>(methodInfo, value => EditorGUILayout.CurveField(value));
            OnDrawInspectorElements<Bounds>(methodInfo, value => EditorGUILayout.BoundsField(value));
            OnDrawInspectorElements<double>(methodInfo, value => EditorGUILayout.DoubleField(value));
        }
    }

    private void OnDrawInspectorButtons(MethodInfo methodInfo)
    {
        InspectorMethodAttribute attribute = (InspectorMethodAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(InspectorMethodAttribute));
        if (attribute == null)
        {
            return;
        }

        if(methodInfo.GetParameters().Length != 0)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(methodInfo.Name));

        if (GUILayout.Button("Invoke"))
        {
            foreach (var t in targets)
            {
                methodInfo.Invoke(t, null);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void OnDrawInspectorElements<T>(MethodInfo methodInfo, Func<T, T> func)
    {
        InspectorMethodAttribute attribute = (InspectorMethodAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(InspectorMethodAttribute));
        if (attribute == null)
        {
            return;
        }

        if(methodInfo.GetParameters().Length == 0 || methodInfo.GetParameters().Length > 1)
        {
            return;
        }

        if(methodInfo.GetParameters()[0].ParameterType != typeof(T))
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(methodInfo.Name));

        if(!m_parameterDatas.ContainsKey(methodInfo))
        {
            m_parameterDatas.Add(methodInfo, null);
        }

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();

        object[] parameterDatas = m_parameterDatas[methodInfo];

        if (parameterDatas == null || parameterDatas.Length != parameterInfos.Length)
        {
            parameterDatas = new object[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                parameterDatas[i] = default(T);
            }

            m_parameterDatas[methodInfo] = parameterDatas;
        }

        for (int i = 0; i < parameterInfos.Length; i++)
        {
            m_parameterDatas[methodInfo][i] = func((T)m_parameterDatas[methodInfo][i]);
        }

        if (GUILayout.Button("Invoke"))
        {
            foreach (var t in targets)
            {
                methodInfo.Invoke(t, m_parameterDatas[methodInfo]);
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif