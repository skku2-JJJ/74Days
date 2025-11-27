using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Data")]
public class ResourceData : ScriptableObject
{
    public ResourceType Type;       
    public string DisplayName;    
    public Sprite Icon;
    
    [TextArea] public string Description; 
}