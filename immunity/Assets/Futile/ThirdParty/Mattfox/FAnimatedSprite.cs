using UnityEngine;
using System;
using System.Collections.Generic;

public class FAnimatedSprite : FSprite {
	
	protected bool _pause = false;
	protected bool _finished = false;
	protected int _finishedCount = 0;
	protected float _time = 0;
	protected int _currentFrame = 0;
	
	protected string _baseName;
	protected string _baseExtension;
	
	protected List<FAnimation> _animations;
	protected string _defaultAnim;
	protected FAnimation _nextAnim;

	
	protected FAnimation _currentAnim;
	
	[Obsolete("use FAnimatedSprite(string elementBase) instead")]
	public FAnimatedSprite  (string elementBase, string elementExtension) {
		ListenForUpdate(Update);
		
		_baseName = elementBase;
		
		// default to first frame, no animation
		Init(FFacetType.Quad, Futile.atlasManager.GetElementWithName(_baseName+"_1"),1); // expects individual frames, in convention of NAME_#.EXT
		_isAlphaDirty = true;
		UpdateLocalVertices();
		
		_animations = new List<FAnimation>();
	}
	
	public FAnimatedSprite (string elementBase) : base() 
	{
		ListenForUpdate(Update);
		
		_baseName = elementBase;
		
		// default to first frame, no animation
		Init(FFacetType.Quad, Futile.atlasManager.GetElementWithName(_baseName+"_1"),1); // expects individual frames, in convention of NAME_#.EXT
		_isAlphaDirty = true;
		UpdateLocalVertices();
		
		_animations = new List<FAnimation>();
	}
	
	// Update is called once per frame
	virtual public void Update () {
		if (_currentAnim != null && !_pause) {
			_time += Time.deltaTime;
			
			while(_time > (float)_currentAnim.delay / 1000.0f)
			{ // loop this way will skip frames if needed
				_currentFrame++;
				if(_currentFrame >= _currentAnim.totalFrames) 
				{
					_finishedCount++;
					if(_currentAnim	.looping) 
					{
						_currentFrame = 0;	
					} 
					else 
					{
						_currentFrame = _currentAnim.totalFrames - 1;
					}
				}
				
				element = Futile.atlasManager.GetElementWithName(_currentAnim.filename+"_"+_currentAnim.frames[_currentFrame]);
				
				_time -= (float)_currentAnim.delay / 1000.0f;
				
			}
		}
	}
	
	public void addAnimation(FAnimation anim) {
		_animations.Add(anim);
		
		if (_currentAnim == null) {
			_currentAnim = anim;
			_currentFrame = 0;
			_pause = false;
		}
	}
	
	public void setDefaultAnimation(string animName) {
		_defaultAnim = animName;
	}
	
	public void play(string animName, bool forced=false) {
		// check if we are given the same animation that is currently playing
		if (_currentAnim.name == animName) {
			if (forced) {
				_finishedCount = 0;
				_currentFrame = 0;
				_time = 0;
								
				// redraw
				element = Futile.atlasManager.GetElementWithName(_currentAnim.filename+"_"+_currentAnim.frames[0]);
			}
			return;
		}
		
		// find the animation with the name given, no change if not found
		foreach (FAnimation anim in _animations) {
			if (anim.name.Equals(animName, StringComparison.Ordinal)) {
				//Debug.Log("Playing " + anim.name);
				_finishedCount = 0;
				_currentAnim = anim;
				_currentFrame = 0;
				_time = 0;
					
				// force redraw to first frame
				element = Futile.atlasManager.GetElementWithName(_currentAnim.filename+"_"+anim.frames[0]);
					
				break;
			}
		}
	}
	
	public void pause(bool forced=false) {
		if (forced) {
			_pause = true;
		} else {
			_pause = !_pause;
		}
	}
	
	public string baseName {
		get { return _baseName; }
		set { _baseName = value; }
	}
	
	public FAnimation currentAnim {
		get { return _currentAnim; }
	}
	
	public int currentFrame {
		get {
			return currentAnim.frames[_currentFrame];
		}
	}
	
	public bool isPaused {
		get { return _pause; }
	}
	
	public int FinishedCount {
		get { return _finishedCount; }	
	}
	
	[Obsolete("baseExtension is unnecessary and unused")]
	public string baseExtension {
		get { return _baseExtension; }
		set { _baseExtension = value; }
	}
}
