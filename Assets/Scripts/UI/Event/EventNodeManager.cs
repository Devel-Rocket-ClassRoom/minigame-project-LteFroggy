using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EventNodeManager : MonoBehaviour
{
    private const string k_EventResourcePath = "Datas/Events";

    [SerializeField]
    private Image _backgroundImage;

    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _dialogueText;

    [SerializeField]
    private TextMeshProUGUI _resultText;

    [SerializeField]
    private Transform _choiceRoot;

    [SerializeField]
    private Button _choiceButtonPrefab;

    [SerializeField]
    private Button _continueButton;

    private bool _completed;

    private void OnEnable()
    {
        _completed = false;
        if (_continueButton != null)
        {
            _continueButton.onClick.AddListener(Complete);
            _continueButton.gameObject.SetActive(false);
        }

        if (_resultText != null)
        {
            _resultText.text = string.Empty;
            _resultText.gameObject.SetActive(false);
        }

        LoadRandomEvent();
    }

    private void OnDisable()
    {
        if (_continueButton != null)
            _continueButton.onClick.RemoveListener(Complete);
        ClearChoiceButtons();
    }

    private void LoadRandomEvent()
    {
        EventConfig[] events = Resources.LoadAll<EventConfig>(k_EventResourcePath);
        if (events == null || events.Length == 0)
        {
            ShowResult(
                "\uC9C4\uD589 \uAC00\uB2A5\uD55C \uC774\uBCA4\uD2B8\uAC00 \uC5C6\uC2B5\uB2C8\uB2E4."
            );
            return;
        }

        ApplyEvent(events[Random.Range(0, events.Length)]);
    }

    private void ApplyEvent(EventConfig eventConfig)
    {
        ClearChoiceButtons();

        if (_backgroundImage != null)
        {
            _backgroundImage.sprite = eventConfig.Background;
            _backgroundImage.preserveAspect = true;
            _backgroundImage.enabled = eventConfig.Background != null;
        }

        if (_titleText != null)
            _titleText.text = eventConfig.Title;
        if (_dialogueText != null)
            _dialogueText.text = eventConfig.Dialogue;
        if (_choiceRoot != null)
            _choiceRoot.gameObject.SetActive(true);
        if (_choiceButtonPrefab != null)
            _choiceButtonPrefab.gameObject.SetActive(false);

        if (eventConfig.Choices == null || eventConfig.Choices.Count == 0)
        {
            ShowResult("\uC774\uBCA4\uD2B8\uAC00 \uC885\uB8CC\uB418\uC5C8\uC2B5\uB2C8\uB2E4.");
            return;
        }

        foreach (EventChoiceConfig choice in eventConfig.Choices)
            CreateChoiceButton(choice);
    }

    private void CreateChoiceButton(EventChoiceConfig choice)
    {
        if (_choiceRoot == null || _choiceButtonPrefab == null)
            return;

        Button button = Instantiate(_choiceButtonPrefab, _choiceRoot);
        button.gameObject.SetActive(true);
        button.interactable = CanSelect(choice);

        SetChoiceTexts(button, choice);
        EventChoiceConfig capturedChoice = choice;
        button.onClick.AddListener(() => OnChoiceSelected(capturedChoice));
    }

    private void SetChoiceTexts(Button button, EventChoiceConfig choice)
    {
        var choiceText = FindText(button.transform, "ChoiceText");
        var previewText = FindText(button.transform, "PreviewText");
        var texts = button.GetComponentsInChildren<TextMeshProUGUI>(true);

        if (choiceText == null && texts.Length > 0)
            choiceText = texts[0];
        if (previewText == null && texts.Length > 1)
            previewText = texts[1];

        if (choiceText != null)
            choiceText.text = choice.ChoiceText;
        if (previewText != null)
            previewText.text = BuildPreviewText(choice);
    }

    private static TextMeshProUGUI FindText(Transform root, string name)
    {
        foreach (var text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (text.name == name)
                return text;
        }

        return null;
    }

    private bool CanSelect(EventChoiceConfig choice)
    {
        if (
            choice.DisableWhenNoUnownedRelic
            && NeedsUnownedRelic(choice)
            && GetUnownedRelics().Count == 0
        )
            return false;
        if (
            choice.DisableWhenInsufficientHealth
            && GetHpCost(choice) >= GamePlayData.Instance.CurrentHealth
        )
            return false;
        return true;
    }

    private string BuildPreviewText(EventChoiceConfig choice)
    {
        if (
            choice.DisableWhenInsufficientHealth
            && GetHpCost(choice) >= GamePlayData.Instance.CurrentHealth
            && !string.IsNullOrWhiteSpace(choice.InsufficientHealthPreviewText)
        )
        {
            return choice.InsufficientHealthPreviewText;
        }

        if (
            NeedsUnownedRelic(choice)
            && GetUnownedRelics().Count == 0
            && !string.IsNullOrWhiteSpace(choice.NoRelicPreviewText)
        )
        {
            return choice.NoRelicPreviewText;
        }

        if (!string.IsNullOrWhiteSpace(choice.PreviewText))
            return choice.PreviewText.Replace("{hpCost}", GetHpCost(choice).ToString());

        var preview = new List<string>();
        foreach (EventEffectConfig effect in choice.Effects)
        {
            switch (effect.Type)
            {
                case EventEffectType.LoseHpPercent:
                    preview.Add($"HP -{CalculateHpCost(effect.Value)}");
                    break;
                case EventEffectType.GainRandomUnownedRelic:
                    preview.Add("\uBBF8\uBCF4\uC720 \uC720\uBB3C 1\uAC1C \uD68D\uB4DD");
                    break;
            }
        }

        return preview.Count > 0
            ? string.Join(" / ", preview)
            : "\uC544\uBB34 \uC77C\uB3C4 \uC77C\uC5B4\uB098\uC9C0 \uC54A\uC2B5\uB2C8\uB2E4.";
    }

    private void OnChoiceSelected(EventChoiceConfig choice)
    {
        if (_completed || !CanSelect(choice))
            return;

        _completed = true;
        EventResultContext context = ApplyEffects(choice);
        ShowResult(BuildResultText(choice, context));
    }

    private EventResultContext ApplyEffects(EventChoiceConfig choice)
    {
        var context = new EventResultContext();
        foreach (EventEffectConfig effect in choice.Effects)
        {
            switch (effect.Type)
            {
                case EventEffectType.LoseHpPercent:
                    ApplyHpLoss(effect.Value, context);
                    break;
                case EventEffectType.GainRandomUnownedRelic:
                    ApplyRandomRelic(context);
                    break;
            }
        }

        return context;
    }

    private static void ApplyHpLoss(int percent, EventResultContext context)
    {
        var data = GamePlayData.Instance;
        int cost = CalculateHpCost(percent);
        int minimumHealth = data.CurrentHealth > 0 ? 1 : 0;
        int nextHealth = Mathf.Max(minimumHealth, data.CurrentHealth - cost);
        context.HpLost += data.CurrentHealth - nextHealth;
        data.SetHealth(nextHealth);
    }

    private static int CalculateHpCost(int percent)
    {
        return Mathf.CeilToInt(GamePlayData.Instance.MaxHealth * (percent / 100f));
    }

    private static void ApplyRandomRelic(EventResultContext context)
    {
        List<RelicBase> candidates = GetUnownedRelics();
        if (candidates.Count == 0)
        {
            context.NoRelicCandidates = true;
            return;
        }

        RelicBase relic = candidates[Random.Range(0, candidates.Count)];
        GamePlayData.Instance.AddRelic(relic);
        context.GainedRelic = relic;
    }

    private static List<RelicBase> GetUnownedRelics()
    {
        var ownedIds = new HashSet<string>();
        foreach (RelicBase relic in GamePlayData.Instance.Relics)
        {
            if (relic != null)
                ownedIds.Add(relic.relicId);
        }

        var result = new List<RelicBase>();
        foreach (RelicBase relic in GamePlayData.AllLoadoutRelics)
        {
            if (relic != null && !ownedIds.Contains(relic.relicId))
                result.Add(relic);
        }

        return result;
    }

    private static bool NeedsUnownedRelic(EventChoiceConfig choice)
    {
        foreach (EventEffectConfig effect in choice.Effects)
        {
            if (effect.Type == EventEffectType.GainRandomUnownedRelic)
                return true;
        }

        return false;
    }

    private static int GetHpCost(EventChoiceConfig choice)
    {
        int total = 0;
        foreach (EventEffectConfig effect in choice.Effects)
        {
            if (effect.Type == EventEffectType.LoseHpPercent)
                total += CalculateHpCost(effect.Value);
        }

        return total;
    }

    private static string BuildResultText(EventChoiceConfig choice, EventResultContext context)
    {
        if (context.NoRelicCandidates && !string.IsNullOrWhiteSpace(choice.NoRelicResultText))
            return FormatText(choice.NoRelicResultText, context);
        if (!string.IsNullOrWhiteSpace(choice.ResultText))
            return FormatText(choice.ResultText, context);

        var lines = new StringBuilder();
        if (context.HpLost > 0)
            lines.AppendLine($"HP\uB97C {context.HpLost} \uC783\uC5C8\uC2B5\uB2C8\uB2E4.");
        if (context.GainedRelic != null)
            lines.AppendLine(
                $"\uC720\uBB3C [{context.GainedRelic.displayName}]\uC744 \uD68D\uB4DD\uD588\uC2B5\uB2C8\uB2E4."
            );
        if (context.NoRelicCandidates)
            lines.AppendLine(
                "\uC774\uBBF8 \uBAA8\uB4E0 \uC720\uBB3C\uC744 \uBCF4\uC720 \uC911\uC785\uB2C8\uB2E4."
            );
        if (lines.Length == 0)
            lines.Append(
                "\uC544\uBB34 \uC77C\uB3C4 \uC77C\uC5B4\uB098\uC9C0 \uC54A\uC558\uC2B5\uB2C8\uB2E4."
            );

        return lines.ToString().TrimEnd();
    }

    private static string FormatText(string text, EventResultContext context)
    {
        string relicName =
            context.GainedRelic != null ? context.GainedRelic.displayName : "\uC5C6\uC74C";
        string relicDescription =
            context.GainedRelic != null ? context.GainedRelic.effectDescription : string.Empty;

        return text.Replace("{hpLost}", context.HpLost.ToString())
            .Replace("{relicName}", relicName)
            .Replace("{relicDescription}", relicDescription);
    }

    private void ShowResult(string message)
    {
        if (_choiceRoot != null)
            _choiceRoot.gameObject.SetActive(false);
        if (_resultText != null)
        {
            _resultText.text = message;
            _resultText.gameObject.SetActive(true);
        }

        if (_continueButton != null)
        {
            _continueButton.gameObject.SetActive(true);
            return;
        }

        Complete();
    }

    private void ClearChoiceButtons()
    {
        if (_choiceRoot == null || _choiceButtonPrefab == null)
            return;

        foreach (Transform child in _choiceRoot)
        {
            if (child.gameObject == _choiceButtonPrefab.gameObject)
                continue;
            Destroy(child.gameObject);
        }
    }

    private void Complete()
    {
        if (_continueButton != null)
            _continueButton.interactable = false;
        GameEvents.NodeCompleted();
    }

    private class EventResultContext
    {
        public int HpLost;
        public RelicBase GainedRelic;
        public bool NoRelicCandidates;
    }
}
