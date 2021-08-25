using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{

	public Sound[] sounds;

    void Awake(){
   		foreach(Sound s in sounds){
   			s.source = gameObject.AddComponent<AudioSource>();
   			s.source.clip = s.clip;

   			s.source.volume = s.volume;
   			s.source.pitch = s.pitch;
   			s.source.loop = s.loop;
   		}
    }

   	public void Play(string name, float vol = 0){
   		Sound s = Array.Find(sounds, sound => sound.name == name);
   		if(s == null){
   			Debug.LogWarning("Sound: " + name + " not found!");
   			return;
   		}
        if(vol != 0) s.source.volume = vol;
   		s.source.Play();
   	}

   	public void Stop(string name){
   		Sound s = Array.Find(sounds, sound => sound.name == name);
   		if(s == null){
   			Debug.LogWarning("Sound: " + name + " not found!");
   			return;
   		}
   		s.source.Stop();
   	}

   	public bool isPlaying(string name){
   		Sound s = Array.Find(sounds, sound => sound.name == name);
   		if(s == null){
   			Debug.LogWarning("Sound: " + name + " not found!");
   			return true;
   		}
   		return s.source.isPlaying;
   	}
}
