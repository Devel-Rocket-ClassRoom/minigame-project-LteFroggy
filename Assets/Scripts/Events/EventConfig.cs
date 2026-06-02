using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Event Config")]
public class EventConfig : ScriptableObject
{
    public Sprite Background;
    public string Title;

    [TextArea(3, 8)]
    public string Dialogue;
    public List<EventChoiceConfig> Choices = new();
}

[Serializable]
public class EventChoiceConfig
{
    public string ChoiceText;

    [TextArea(1, 3)]
    public string PreviewText;

    [TextArea(1, 3)]
    public string NoRelicPreviewText;

    [TextArea(1, 3)]
    public string InsufficientHealthPreviewText;

    [TextArea(1, 3)]
    public string ResultText;

    [TextArea(1, 3)]
    public string NoRelicResultText;
    public bool DisableWhenNoUnownedRelic;
    public bool DisableWhenInsufficientHealth;
    public List<EventEffectConfig> Effects = new();
}

[Serializable]
public class EventEffectConfig
{
    public EventEffectType Type;

    [Min(0)]
    public int Value;
}

public enum EventEffectType
{
    None,
    LoseHpPercent,
    GainRandomUnownedRelic,
}
