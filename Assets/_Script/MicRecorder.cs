using UnityEngine;
using System;
using Microphone = FrostweepGames.MicrophonePro.Microphone;
using System.Collections;

#if unusedjunk
using NAudio.Wave;

public class MicRecorder : MonoBehaviour
{
    private WaveInEvent waveIn;
    private MemoryStream memoryStream;
    private WaveFileWriter writer;

    bool isRecording = false;
    string outputFileName = "output.wav";

    void Start()
    {
    }

    void StartRecording()
    {
        Debug.Log("Starting recording system");
        waveIn = new WaveInEvent();
        waveIn.DeviceNumber = 0; // Use the default microphone
        waveIn.WaveFormat = new WaveFormat(44100, 16, 1); // 44100Hz, Mono
        
        writer = new WaveFileWriter(outputFileName, waveIn.WaveFormat);

        waveIn.DataAvailable += (sender, e) =>
        {
            if (isRecording)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        };

        // Start recording audio from the microphone
        waveIn.StartRecording();
        isRecording = true;

        Console.WriteLine("Recording audio. Press S to stop recording...");

    }
    private void Update()
    {

        if (!isRecording && Input.GetKey(KeyCode.M))
        {
            Debug.Log("Recording started");
            StartRecording();
        }
        
        if (isRecording && !Input.GetKey(KeyCode.M))
        {
            Debug.Log("Recording stopped");
            waveIn.StopRecording();
            writer.Close();
            isRecording = false;

            AIManager aiScript = GetComponent<AIManager>();
            //OPTIMIZE: Pass the .wav bytes directly instead of writing/reading from an actual file?
            aiScript.ProcessMicAudioByFileName(outputFileName);

        }
       
    }
}
#else

public class MicRecorder : MonoBehaviour
{
    private AudioClip audioClip;
    private int recordingLength = 0;

    bool isRecording = false;
    
    // added variable
    private string selectedDevice;

    int minFreq;
    int maxFreq;

    /*
    void Start()
    {
    }*/

    // Added Method for WebGL using FrostSweep (Max Fik added it on 03.06.2024)
    IEnumerator Start()
    {
        // Request permission to use the microphone
        Microphone.RequestPermission();

        // Wait until the user has granted permission
        yield return new WaitUntil(() => Microphone.devices.Length > 0);

        // Select the microphone device that the user selected in the browser popup
        SelectActiveMicrophone();

        //  set highest frequencies
        Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);
    }

    // Added method by Max Fink
    // a method for selecting the microphone in WebGL
    void SelectActiveMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            selectedDevice = Microphone.devices[0]; // Assume the selected microphone is the first one listed
            Debug.Log("Selected Device: " + selectedDevice);
        }
        else
        {
            Debug.LogError("No microphone devices found!");
        }
    }

    public void StartRecording()
    {
        Debug.Log("Starting recording system");
        // Start recording audio from the microphone
        // audioClip = Microphone.Start(null, false, 30, 22050);

        int sampleRate = maxFreq == 0 ? 44100 : maxFreq;
        audioClip = Microphone.Start(selectedDevice, false, 30, sampleRate);

        isRecording = true;
        Console.WriteLine("Recording audio...");
    }

    public void Destroy()
    {
        if (IsRecording())
        {
            Debug.Log("Recording stopped");
            //Microphone.End(null);
            Microphone.End(selectedDevice);

            isRecording = false;
        }
    }
    public void StopRecordingAndProcess(string outputFileName)
    {

        if (!isRecording)
            return;

        Debug.Log("Recording stopped");
        // Microphone.End(null);
        Microphone.End(selectedDevice);
        isRecording = false;

        recordingLength = Mathf.RoundToInt(audioClip.length * audioClip.channels * audioClip.frequency) * 2;

        // Convert AudioClip data to a WAV file and save it
        float[] audioData = new float[recordingLength];
        audioClip.GetData(audioData, 0);

        SavWav.Save(outputFileName, audioClip, true);

        AIManager aiScript = GetComponent<AIManager>();
        aiScript.ProcessMicAudioByFileName(outputFileName);
    }

   public bool IsRecording() { return isRecording; }
    private void Update()
    {

        /*
        if (!isRecording && Input.GetKey(KeyCode.M))
        {
            Debug.Log("Recording started");
            StartRecording();
        }

        if (isRecording && !Input.GetKey(KeyCode.M))
        {
           StopRecordingAndProcess();
        }*/
        
    }
}
#endif