using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Resource Data")]
public class ResourceData : ScriptableObject
{
    public ResourceType Type;       
    public string DisplayName;    
    public Sprite Icon;
    
    [TextArea] public string Description; 
}