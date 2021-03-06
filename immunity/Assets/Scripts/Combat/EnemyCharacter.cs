﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyCharacter : FAnimatedSprite {
	
	public enum BehaviorType { 
		IDLE,
		MOVE_TOWARDS_PLAYER,
		MOVE_AWAY_FROM_PLAYER,
		PUNCH,
		SPAWN_SWARM,
		BLOCK,
		HIT
	}
	
	public const int MAX_HEALTH = 100;
	private int health_ = MAX_HEALTH;
	
	private HealthBar health_bar_;
	
	public BehaviorType curr_behavior_ = BehaviorType.IDLE;
	public float behavior_start_time_;
	
	public float speed_ = .0025f;
	
	public int NUM_SPAWNED_SWARM = 4;
	public int spawn_count = 0;	
		
	public EnemyCharacter(HealthBar health_bar) : base("punchy_idle")
	{
		ListenForUpdate(HandleUpdate);
		
		this.health_bar_ = health_bar;
		
		this.anchorX = .65f;
		this.anchorY = .6f;
		
		// set up animations
		// -------------------
		
		// idle animation
		int[] idle_frames = {1, 2, 3, 4};
		FAnimation idle_animation = new FAnimation("idle", "punchy_idle", idle_frames, 150, true);
		base.addAnimation(idle_animation);
		
		// punch animation
		int[] punch_frames = {1, 2, 3, 4, 5, 6, 7};
		FAnimation punch_animation = new FAnimation("punch", "punchy_punch", punch_frames, 100, false);
		base.addAnimation(punch_animation);
		
		// taking damage animation
		int[] hit_frames = {1, 2, 3, 4, 5, 6};
		FAnimation hit_animation = new FAnimation("hit", "punchy_hit", hit_frames, 100, false);
		base.addAnimation(hit_animation);
		
		// blocking animation
		int[] block_frames = {1, 2, 3, 4, 5};
		FAnimation block_animation = new FAnimation("block", "punchy_block", block_frames, 100, false);
		base.addAnimation(block_animation);
		
		// walking animation
		int[] walk_frames = {1, 2, 3, 4};
		FAnimation walk_animation = new FAnimation("walk", "punchy_walk", walk_frames, 100, true);
		base.addAnimation(walk_animation);
		
		int[] backwards_walk_frames = {4, 3, 2, 1};
		FAnimation backwards_walk_animation = new FAnimation("backwards_walk", "punchy_walk", backwards_walk_frames, 100, true);
		base.addAnimation(backwards_walk_animation);
		
		int[] death_frames = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
		FAnimation death_animation = new FAnimation("death", "punchy_death", death_frames, 100, false);
		base.addAnimation(death_animation);
		
		base.setDefaultAnimation("idle");
				
		y = -Futile.screen.halfHeight + height/2.0f + 25.0f;
		x = Futile.screen.halfWidth - width/2.0f - 150.0f;
		
	}
	
	// TODO: refactor this into a base class (Character class)
	public void ChangeHealth(int health_delta)
	{
		health_ += health_delta;
		if(health_ > MAX_HEALTH)
			health_ = MAX_HEALTH;
		if(health_ < 0)
			health_ = 0;
		health_bar_.Percentage = (float)health_/(float)MAX_HEALTH;
	}
	
	public int Health
	{
		get { return this.health_; }	
	}
	
	public bool isDead
	{
		get { return (health_ <= 0); }	
	}
	
	void HandleUpdate()
	{
		base.Update();
		
		// if not currently doing anything choose something to do with some probability
		if(curr_behavior_ == BehaviorType.IDLE)
		{
			float behavior_selection = Random.value;
			
			if(behavior_selection < 0.1f)
			{
				// switch to move_towards_player behavior
				curr_behavior_ = BehaviorType.MOVE_TOWARDS_PLAYER;
				Debug.Log("Behavior: Move towards player");
			}
			else if(behavior_selection < .2f)
			{
				// switch to move_away_from_player behavior
				curr_behavior_ = BehaviorType.MOVE_AWAY_FROM_PLAYER;
				Debug.Log("Behavior: Move away from player");
			}
			else if(behavior_selection < .3f)
			{
				// switch to punch behavior	
				curr_behavior_ = BehaviorType.PUNCH;
				Debug.Log("Behavior: Punch");
			}
			else if(behavior_selection < .4f)
			{
				// switch to block behavior
				curr_behavior_ = BehaviorType.BLOCK;
				Debug.Log("Behavior: Block");
			}
			else if(behavior_selection < .45f)
			{
				// switch to swarm behavior	
				curr_behavior_ = BehaviorType.SPAWN_SWARM;
				spawn_count = 0;
				Debug.Log("Behavior: spawn swarm");
			}
			behavior_start_time_ = Time.time;
		}
	}
	
	public List<Rect> getCollisionRects()
	{
		List<Rect> rects = new List<Rect>();
		if(curr_behavior_ == BehaviorType.PUNCH)
			rects.Add(localRect.CloneAndScaleThenOffset(scaleX, scaleY, x, y));
		else
			rects.Add(localRect.CloneAndScaleThenOffset(scaleX*.5f, scaleY*.5f, x, y));
		
		return rects;
	}
	
	public float width
	{
		get { return base.width*.595f; }	
	}
	
	public float height
	{
		get { return base.height*.993f; }	
	}
}
