using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class MidiPlayer : MonoSingleton<MidiPlayer>
{
    #region Const parameters
    #endregion

    #region Editor paramters
    [SerializeField]
    private string midiPath;
    [SerializeField]
    private string soundPath;

    [SerializeField]
    private bool isTestMode;
    [SerializeField]
    private string testMidiName; 

    #endregion

    #region Normal paramters
    private Dictionary<string, MidiData> midiDataByName;
    private Dictionary<string, AudioClip> clipByName;

    private Dictionary<int, MidiData> midiDataById;
    private Dictionary<int, AudioClip> clipById;

    private int midiCount;

    private List<NoteData> listNoteData;
    private float curTimes;
    private bool isPlay;
    private int curNote;

    private AudioSource audioSoure;

    public event System.Action<int, float> NotePlay = delegate { };
    #endregion

    #region Encapsulate
    #endregion

    public void Initialize()
    {
        midiDataByName = new Dictionary<string, MidiData>();
        clipByName = new Dictionary<string, AudioClip>();

        midiDataById = new Dictionary<int, MidiData>();
        clipById = new Dictionary<int, AudioClip>();

        TextAsset[] assets = Resources.LoadAll<TextAsset>(midiPath);

        for (int i = 0; i < assets.Length; i++)
        {
            var clip = Resources.Load<AudioClip>(soundPath + Path.DirectorySeparatorChar + assets[i].name);
            if (clip != null)
            {
                MidiData data = new MidiData();
                MidiParser.ParseNotesData(assets[i].bytes, ref data);

                midiDataByName.Add(assets[i].name, data);
                clipByName.Add(assets[i].name, clip);

                midiDataById.Add(i, data);
                clipById.Add(i, clip);
            }
            else
            {
                Debug.LogError("[MidiPlayer] Missing sound file :" + assets[i].name);
            }
        }

        audioSoure = GetComponent<AudioSource>();

        midiCount = assets.Length;
    }

    public void Release()
    {
    }

    public void Refresh()
    {
    }

    public void UpdateStep(float deltaTime)
    {
        if (!isPlay)
            return;

        for (int i = curNote; i < listNoteData.Count; i++)
        {
            NoteData data = listNoteData[i];
            if(data.timeAppear <= curTimes)
            {
                curNote = i + 1;

                if(data.nodeID >= 72 && data.nodeID <= 76)
                {
                    NotePlay(data.nodeID, data.duration);
                    //Debug.Log(string.Format("Times: {0}, note: {1} , duration {2}", curTimes, data.nodeID, data.duration));
                }

            }
            else
            {
                if (curNote == listNoteData.Count)
                    isPlay = false;
                break;
            }
        }

        curTimes += deltaTime;
    }

    public void Awake()
    {
        Initialize();
    }

    public void RandomMidi(out float midiTimes)
    {
        MidiData data = null;
        AudioClip clip = null;

        if (isTestMode)
        {
            data = midiDataByName[testMidiName];
            clip = clipByName[testMidiName];
        }
        else
        {
            int id = UnityEngine.Random.Range(0, midiCount);
            data = midiDataById[id];
            clip = clipById[id];
        }

        for (int i = 0; i < data.notesData.Count; i++)
        {
            if (data.notesData[i].Count > 0)
            {
                listNoteData = data.notesData[i];
                break;
            }
        }

        curTimes = 0.0f;
        curNote = 0;

        isPlay = true;

        audioSoure.clip = clip;
        audioSoure.Play((ulong)(44100 * 0.1f));

        midiTimes = listNoteData[listNoteData.Count - 1].timeAppear + listNoteData[listNoteData.Count - 1].duration;
    }

}
