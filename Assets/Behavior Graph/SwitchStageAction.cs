using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SwitchStage", story: "[Agent] switches [Stage] to [another]", category: "Action", id: "a150c23395f389f65bffc2c1eb2ef6bd")]
public partial class SwitchStageAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> Stage;
    [SerializeReference] public BlackboardVariable<int> Another;

    [SerializeReference] public BlackboardVariable<GameObject> GlobalLightSwitch;

    private Light2D light2D;

    protected override Status OnStart()
    {
        if (!GlobalLightSwitch.Value.TryGetComponent(out light2D))
        {
            Debug.LogError("Could not find light source");
            return Status.Failure;
        }

        Another.Value++;
        Color colorToChange;
        if (light2D != null && ColorUtility.TryParseHtmlString("#FF2412", out colorToChange))  // Hex color for #FF4B4B (a redish color)
        {
            ChangeLightColor(colorToChange, 2f);
        }
        else
        {
            Debug.LogError("Invalid hex color");
        }
        return Status.Running;
    }

    void ChangeLightColor(Color targetColor, float duration)
    {
        Color startColor = light2D.color;
        DOVirtual.Color(startColor, targetColor, 10, (value) =>
        {
            light2D.color = value;
        });
    }

    protected override Status OnUpdate()
    {
        Stage.Value = false;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

