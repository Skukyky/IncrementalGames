using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Animation d'invocation : un caillou tombe, le joueur le tape plusieurs fois (il tremble et
/// s'assombrit), il éclate et révèle le personnage obtenu. Toucher l'écran ferme la fenêtre.
/// Tout est animé en temps réel (indépendant de Time.timeScale).
/// </summary>
public class GachaReveal : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Button backdropButton;

    [Header("Caillou")]
    [SerializeField] private RectTransform rock;
    [SerializeField] private Image rockImage;
    [SerializeField] private Button rockButton;
    [SerializeField] private int minTaps = 3;
    [SerializeField] private int maxTaps = 5;

    [Header("Révélation")]
    [SerializeField] private RectTransform revealRoot;
    [SerializeField] private RectTransform glow;
    [SerializeField] private Image characterIcon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text detailText;
    [SerializeField] private TMP_Text hintText;

    public bool IsPlaying { get; private set; }

    private Vector2 _rockHome;
    private int _tapsLeft;
    private int _tapsTotal;
    private bool _canClose;
    private Action _onClosed;
    private Coroutine _hitRoutine;

    private void Awake()
    {
        _rockHome = rock.anchoredPosition;
        rockButton.onClick.AddListener(OnRockTapped);
        backdropButton.onClick.AddListener(OnBackdropTapped);
    }

    private void Update()
    {
        if (glow != null && glow.gameObject.activeInHierarchy)
        {
            glow.Rotate(0f, 0f, -40f * Time.unscaledDeltaTime);
            float pulse = 1f + 0.06f * Mathf.Sin(Time.unscaledTime * 4f);
            glow.localScale = new Vector3(pulse, pulse, 1f);
        }
    }

    public void Play(CharacterData character, float chance, bool isNew, Action onClosed = null)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        IsPlaying = true;
        _onClosed = onClosed;
        _canClose = false;
        _tapsTotal = Mathf.Max(1, Random.Range(minTaps, maxTaps + 1));
        _tapsLeft = _tapsTotal;

        characterIcon.sprite = character.icon != null ? character.icon : character.sprite1;
        nameText.text = character.CharacterName;
        detailText.text = (chance * 100f).ToString("0.#") + " %  -  " + (isNew ? "Nouveau !" : "Doublon");
        detailText.color = isNew ? new Color(1f, 0.85f, 0.3f) : new Color(0.8f, 0.85f, 0.9f);

        rock.gameObject.SetActive(true);
        rock.anchoredPosition = _rockHome + new Vector2(0f, 1400f);
        rock.localScale = Vector3.one;
        rock.localRotation = Quaternion.identity;
        rockImage.color = Color.white;
        rockButton.interactable = false;
        revealRoot.gameObject.SetActive(false);
        hintText.text = "";

        StartCoroutine(Intro());
    }

    private IEnumerator Intro()
    {
        yield return Fade(0f, 1f, 0.2f);

        // Chute avec rebond
        Vector2 from = rock.anchoredPosition;
        yield return Animate(0.6f, t =>
        {
            rock.anchoredPosition = Vector2.LerpUnclamped(from, _rockHome, EaseOutBounce(t));
        });

        rockButton.interactable = true;
        hintText.text = "Touchez le caillou !";
    }

    private void OnRockTapped()
    {
        if (_tapsLeft <= 0)
            return;

        _tapsLeft--;
        if (_hitRoutine != null) StopCoroutine(_hitRoutine);

        if (_tapsLeft > 0)
        {
            hintText.text = "Encore " + _tapsLeft + (_tapsLeft > 1 ? " coups !" : " coup !");
            _hitRoutine = StartCoroutine(Hit());
        }
        else
        {
            rockButton.interactable = false;
            hintText.text = "";
            _hitRoutine = StartCoroutine(Break());
        }
    }

    /// <summary>Un coup : écrasement, tremblement, rotation, et le caillou s'assombrit un peu plus.</summary>
    private IEnumerator Hit()
    {
        float damage = 1f - (float)_tapsLeft / _tapsTotal;
        rockImage.color = Color.Lerp(Color.white, new Color(0.55f, 0.5f, 0.5f), damage);
        float angle = Random.Range(-10f, 10f) * (0.5f + damage);

        yield return Animate(0.25f, t =>
        {
            float decay = 1f - t;
            float squash = 0.18f * decay * Mathf.Cos(t * Mathf.PI * 3f);
            rock.localScale = new Vector3(1f + squash, 1f - squash, 1f);
            rock.anchoredPosition = _rockHome + new Vector2(Mathf.Sin(t * Mathf.PI * 8f) * 30f * decay * (0.5f + damage), 0f);
            rock.localRotation = Quaternion.Euler(0f, 0f, angle * decay);
        });

        rock.anchoredPosition = _rockHome;
        rock.localScale = Vector3.one;
        rock.localRotation = Quaternion.identity;
    }

    /// <summary>Dernier coup : tremblement violent, éclatement, puis apparition du personnage.</summary>
    private IEnumerator Break()
    {
        yield return Animate(0.4f, t =>
        {
            rock.anchoredPosition = _rockHome + Random.insideUnitCircle * 25f * (0.5f + t);
            rock.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-6f, 6f));
        });

        rock.anchoredPosition = _rockHome;
        yield return Animate(0.25f, t =>
        {
            float s = 1f + 0.6f * t;
            rock.localScale = new Vector3(s, s, 1f);
            Color c = rockImage.color;
            c.a = 1f - t;
            rockImage.color = c;
        });
        rock.gameObject.SetActive(false);

        revealRoot.gameObject.SetActive(true);
        yield return Animate(0.45f, t =>
        {
            float s = EaseOutBack(t);
            revealRoot.localScale = new Vector3(s, s, 1f);
        });

        yield return Wait(0.4f);
        hintText.text = "Touchez pour continuer";
        _canClose = true;
    }

    private void OnBackdropTapped()
    {
        if (!_canClose)
            return;

        _canClose = false;
        StartCoroutine(Close());
    }

    private IEnumerator Close()
    {
        yield return Fade(1f, 0f, 0.2f);
        gameObject.SetActive(false);
        IsPlaying = false;
        _onClosed?.Invoke();
    }

    // --- Outils d'animation ---------------------------------------------------------------

    private IEnumerator Fade(float from, float to, float duration)
    {
        yield return Animate(duration, t => group.alpha = Mathf.Lerp(from, to, t));
    }

    private static IEnumerator Animate(float duration, Action<float> step)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            step(elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        step(1f);
    }

    private static IEnumerator Wait(float duration)
    {
        float end = Time.unscaledTime + duration;
        while (Time.unscaledTime < end)
            yield return null;
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;
        if (t < 1f / d1) return n1 * t * t;
        if (t < 2f / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
        if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }
}
