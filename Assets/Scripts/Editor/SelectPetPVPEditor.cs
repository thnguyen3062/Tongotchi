//using TMPro;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(SelectPetPVP))]
//public class SelectPetPVPEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        SelectPetPVP script = (SelectPetPVP)target;

//        script.PetType = (SelectPetType)EditorGUILayout.EnumPopup("Pet Type", script.PetType);

//        switch (script.PetType)
//        {
//            case SelectPetType.Attack:
//                {
//                    script.FindingMatch = EditorGUILayout.ObjectField("Finding Match", script.FindingMatch, typeof(GameObject), true) as GameObject;
//                    script.FindingTime = EditorGUILayout.ObjectField("Finding Time", script.FindingTime, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
//                    break;
//                }
//            case SelectPetType.Defense:
//                {

//                    break;
//                }
//        }
//    }
//}
