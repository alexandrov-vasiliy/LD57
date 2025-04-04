using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {

    public string LoopName
    {
        get
        {
            var clip = GetComponent<AudioSource>().clip;
            if (clip != null)
            {
                return clip.name;
            }
            else return null;
        }
    }

    public float LoopVolume
    {
        get
        {
            if (source.clip != null)
            {
                return source.volume;
            }
            else
            {
                return 0f;
            }
        }
    }

    public float LoopTimeNormalized
    {
        get
        {
            if (source != null && source.clip != null)
            {
                return 1f - ((source.clip.length - source.time) / source.clip.length);
            }
            else
            {
                return 0f;
            }
        }
    }

    List<AudioClip> SFX = new List<AudioClip>();
    List<AudioClip> Loops = new List<AudioClip>();
    public static AudioController Instance { get; private set; }
    AudioSource source;

    public string InitialSong;
    public float loopOffset;

    public bool Fading { get; set; }

    private string currentSong = "";

    private Dictionary<string, float> limitedFrequencySounds = new Dictionary<string, float>();
    private Dictionary<string, int> lastPlayedSounds = new Dictionary<string, int>();

    private List<AudioMixer> loadedMixers = new List<AudioMixer>();
    private AudioMixerGroup currentSFXMixer;

    private Coroutine skippingCoroutine;

    private const string NULL_INITIAL_SONG = "null";

    void Awake() {
        if (Instance) {
            if (InitialSong != NULL_INITIAL_SONG)
            {
                if (!string.IsNullOrEmpty(InitialSong))
                {
                    Instance.FadeLoopIfNotPlaying(InitialSong, 1f);
                }
                else
                {
                    Instance.currentSong = string.Empty;
                }
            }
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();

        foreach (object o in Resources.LoadAll("Audio/SFX"))
        {
            SFX.Add((AudioClip)o);
        }
        foreach (object o in Resources.LoadAll("Audio/Loops"))
        {
            Loops.Add((AudioClip)o);
        }

        if (InitialSong != NULL_INITIAL_SONG)
        {
            CrossFadeLoop(InitialSong, 1f, 5f, 1f, loopOffset);
        }
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.outputAudioMixerGroup = currentSFXMixer;

        aSource.Play();
        Destroy(tempGO, clip.length);
        return aSource;
    }

    public AudioSource PlaySound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            return null;
        }

        AudioClip sound = SFX.Find(x => x.name == soundName);

        if (sound != null)
        {
            return PlayClipAt(sound, Vector3.zero);
        }

        return null;
    }

    public AudioSource PlaySoundAtLimitedFrequency(string soundName, float frequencyMin, float pitch = 1f, float volume = 1f, string entrySuffix = "") {

        float time = Time.unscaledTime;
        string soundKey = soundName + entrySuffix;

        if (limitedFrequencySounds.ContainsKey(soundKey))
        {
            if (time - frequencyMin > limitedFrequencySounds[soundKey])
            {
                limitedFrequencySounds[soundKey] = time;
                return PlaySoundWithPitch(soundName, pitch, volume);
            }
        }
        else
        {
            limitedFrequencySounds.Add(soundKey, time);
            return PlaySoundWithPitch(soundName, pitch, volume);
        }

        return null;
    }

    public AudioSource PlayRandomSound(string soundPrefix, bool noRepeating = false, bool randomPitch = false)
    {
        List<AudioClip> variations = SFX.FindAll(x => x != null && x.name.StartsWith(soundPrefix + " "));

        string soundId = "";

        if (variations.Count > 0)
        {
            int index = Random.Range(0, variations.Count) + 1;
            if (noRepeating)
            {
                if (!lastPlayedSounds.ContainsKey(soundPrefix))
                {
                    lastPlayedSounds.Add(soundPrefix, index);
                }
                else
                {
                    int breakOutCounter = 0;
                    const int BREAK_OUT_THRESHOLD = 100;
                    while (lastPlayedSounds[soundPrefix] == index && breakOutCounter < BREAK_OUT_THRESHOLD)
                    {
                        index = Random.Range(0, variations.Count) + 1;
                        breakOutCounter++;
                    }

                    if (breakOutCounter >= BREAK_OUT_THRESHOLD - 1)
                    {
                        Debug.Log("Broke out of infinite loop! AudioController.PlayRandomSound");
                    }

                    lastPlayedSounds[soundPrefix] = index;
                }
            }

            soundId = soundPrefix + " " + index;
        }
        else
        {
            soundId = soundPrefix;
        }

        AudioSource randomSound = PlaySoundWithPitch(soundId, randomPitch ? Random.Range(0.9f, 1.1f) : 1f, 1f);

        if (randomSound == null)
        {
            return PlaySoundWithPitch(soundPrefix, 1f, 1f);
        }
        else
        {
            return randomSound;
        }
    }

    public void PlaySoundDelayed(string soundName, float delay, float pitch = 1f, float volume = 1f) {
        StartCoroutine(WaitThenPlaySound(soundName, delay, pitch, volume));
    }

    IEnumerator WaitThenPlaySound(string soundName, float delay, float pitch, float volume) {
        yield return new WaitForSeconds(delay);
        PlaySoundWithPitch(soundName, pitch, volume);
    }

    public AudioSource PlaySoundWithPitch(string soundName, float pitch, float volume) {
        if (!string.IsNullOrEmpty(soundName)) {
            AudioSource a = PlaySound(soundName);
            if (a != null)
            {
                a.pitch *= pitch;
                a.volume *= volume;
            }
            return a;
        } else {
            return null;
        }
    }

    public AudioSource PlaySoundMuffled(string soundName, float cutoff = 300f)
    {
        var sound = PlaySound(soundName);
        var filter = sound.gameObject.AddComponent<AudioLowPassFilter>();
        filter.cutoffFrequency = cutoff;
        return sound;
    }

    public void SetLooping(bool looping)
    {
        source.loop = looping;
    }

    public void PauseLoop() {
        source.Pause();
    }

    public void ResumeLoop(float fadeInSpeed = float.MaxValue) {
        source.UnPause();

        if (!source.isPlaying)
        {
            source.Play();
        }

        source.volume = 0f;
        FadeInLoop(fadeInSpeed);
    }

    public void RestartLoop()
    {
        source.Stop();
        source.Play();
        Fading = false;
    }

    public void StopLoop() {
        source.Stop();
        currentSong = "";
        Fading = false;
    }

    public AudioClip GetLoop(string loopName)
    {
        return Loops.Find(x => x.name == loopName);
    }

    public void SetLoop(string loopName)
    {
        currentSong = loopName;
        if (source != null)
        {
            AudioClip loop = GetLoop(loopName);
            if (loop != null)
            {
                source.Stop();
                source.time = 0f;
                source.clip = loop;
                source.Play();
            }
        }
    }

    public void ClearLoopMixing()
    {
        source.outputAudioMixerGroup = null;
    }

    public void SetLoopMixing(string mixerId, string groupId)
    {
        source.outputAudioMixerGroup = GetMixerGroup(mixerId, groupId);
    }

    public AudioClip GetSFX(string sfxName)
    {
        return SFX.Find(x => x.name == sfxName);
    }

    public void ClearSFXMixing()
    {
        currentSFXMixer = null;
    }
    
    public void SetSFXMixing(string mixerId, string groupId)
    {
        currentSFXMixer = GetMixerGroup(mixerId, groupId);
    }

	public void FadeLoopIfNotPlaying(string loopName, float volume) {
        if (loopName != currentSong)
        {
            SetLoop(loopName);
            SetLoopVolume(volume);
            FadeInLoop(1f);
        }
	}

	public void CrossFadeLoopIfNotPlaying(string loopName, float volume, float speed = 0.5f, float pitch = 1f) {
		if (currentSong != loopName) {
			CrossFadeLoop(loopName, volume, speed, pitch);
		}
	}
	
	public void CrossFadeLoop(string loopName, float volume = 1f, float speed = 5f, float pitch = 1f, float offset = 0f) {
		currentSong = loopName;
		for (int i = 0; i < Loops.Count; i ++) {
			AudioClip wav = Loops[i];
			if (wav.name == loopName) {
				StopAllCoroutines();
				StartCoroutine(Cross(loopName,volume,false, speed, pitch, offset));
				return;
			}
		}
	}
	
	public void FadeOutLoop(float fadeSpeed = float.MaxValue) {
        StopAllCoroutines();
		StartCoroutine(DoFadeToVolume(fadeSpeed, 0f));
	}

	public void FadeInLoop(float fadeSpeed = float.MaxValue, float toVolume = 1f) {
        StopAllCoroutines();
		StartCoroutine(DoFadeToVolume(fadeSpeed, toVolume));
	}

	IEnumerator DoFadeToVolume(float fadeSpeed, float newNormalizedVolume)
	{
        Fading = true;
        if (source.volume > GetAdjustedVolume(newNormalizedVolume))
        {
            //Fading Down
            while (source.volume >= GetAdjustedVolume(newNormalizedVolume))
            {
                source.volume -= GetAdjustedVolume(Time.deltaTime * fadeSpeed);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            //Fading Up
            while (source.volume <= GetAdjustedVolume(newNormalizedVolume))
            {
                source.volume += GetAdjustedVolume(Time.deltaTime * fadeSpeed);
                yield return new WaitForEndOfFrame();
            }
        }
        source.volume = GetAdjustedVolume(newNormalizedVolume);
        Fading = false;
    }

	public void FadeOutThenPlayAfterDelay(string bgm, float delay) {
		FadeOutLoop();
		StartCoroutine(WaitThenPlay(bgm, delay));
	}

	IEnumerator WaitThenPlay(string bgm, float waitTime) {
		yield return new WaitForSeconds(waitTime);
		CrossFadeLoop(bgm, 1f);
	}
	
	IEnumerator Cross (string newLoop, float volume, bool fadeOut, float speed = 5f, float pitch = 1f, float offset = 0f) {
        Fading = true;
		//If a loop is playing, fade it out
		if (source.clip && source.isPlaying) {
			while (source.volume > 0f) {
                source.volume -= (speed * Time.deltaTime);
				yield return new WaitForEndOfFrame();
			}
            source.volume = 0f;
		}
		
		if (!fadeOut) {
            //Fade in the new loop to be played
            SetLoop(newLoop);
			SetLoopVolume(0);
			source.pitch = pitch;
			source.time = offset;
			while (source.volume < GetAdjustedVolume(volume)) {
                source.volume += (speed * Time.deltaTime);
				yield return new WaitForEndOfFrame();
			}
            source.volume = GetAdjustedVolume(volume);
		}
        Fading = false;
	}

    public void SetLoopCutoff(float cutoff)
    {
        var filter = GetComponent<AudioLowPassFilter>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<AudioLowPassFilter>();
        }

        filter.cutoffFrequency = cutoff;
    }

    public void SetLoopVolume(float volume, bool stopFades = false)
    {
        if (stopFades)
        {
            StopAllCoroutines();
        }
        source.volume = volume;
    }

    public void SetLoopVolume(float volume, float speed)
    {
        StopAllCoroutines();
        StartCoroutine(DoFadeToVolume(speed, volume));
    }

    public void SetLoopTimeNormalized(float time)
    {
        if (source.clip != null)
        {
            float newTime = source.clip.length * time;
            if (newTime < source.clip.length)
            {
                source.time = newTime;
            }
        }
    }

    public void StartLoopSkipping(float skipLength)
    {
        StopLoopSkipping();
        skippingCoroutine = StartCoroutine(DoLoopSkipping(source.time, skipLength));
    }

    public void StopLoopSkipping()
    {
        if (skippingCoroutine != null)
        {
            StopCoroutine(skippingCoroutine);
        }
    }

    public void StopCrossFade()
    {
        StopAllCoroutines();
    }

	public AudioMixerGroup GetMixerGroup(string mixerId, string groupId)
	{
		var loadedMixer = loadedMixers.Find (x => x.name == mixerId);
		if (loadedMixer == null)
		{
			loadedMixer = Resources.Load<AudioMixer> ("Audio/Mixing/" + mixerId);
			loadedMixers.Add (loadedMixer);
		}

		if (loadedMixer != null)
		{
			var groups = loadedMixer.FindMatchingGroups (groupId);
			if (groups.Length > 0)
			{
				return groups [0];
			}
		}

		return null;
	}

    public IEnumerator FadeMixerParameter(string mixer, string channel, string parameter, float fadeTo, float fadeLength)
    {
        var mainBGM = GetMixerGroup(mixer, channel);
        float startVolume; mainBGM.audioMixer.GetFloat(parameter, out startVolume);
        float timer = 0f;

        while (timer < fadeLength)
        {
            timer += Time.deltaTime;
            mainBGM.audioMixer.SetFloat(parameter, Mathf.Lerp(startVolume, fadeTo, timer / fadeLength));
            yield return new WaitForEndOfFrame();
        }

        mainBGM.audioMixer.SetFloat(parameter, fadeTo);
    }

    public IEnumerator FadeOutExternalSource(AudioSource externalSource, float fadeSpeed)
    {
        while (externalSource != null && externalSource.volume > 0f)
        {
            externalSource.volume -= Time.deltaTime * fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator DoLoopSkipping(float startTime, float skipLength)
    {
        while (true)
        {
            yield return new WaitForSeconds(skipLength);
            source.time = startTime;
        }
    }

    private float GetAdjustedVolume(float volume)
    {
        return volume;
    }
}