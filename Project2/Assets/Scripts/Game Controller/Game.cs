using UnityEngine;
using System.Collections;

public static class Game {

    // Game Variables
    public const float TIME = 0;
    public const float DOOR_ANIMATION_TIME = 1.3f;
    public const int SPRITE_PPU = 8;

    // Level Name SHootcuts

    public const int LEVEL_LAYER = 8;
    public const int ENEMY_LAYER = 9;
    public const int ALLY_SHOOT_LAYER = 10;
    public const int ENEMY_SHOOT_LAYER = 11;

    // Player Variables
    public const float PLAYER_HURT_TIME = 0.6f;
    public const float PLAYER_DIST_SHOOT = 0.8f;
    public const float PLAYER_INVULNERABILITY_TIME = 1.0f;


    public const float TIME_PLAYER_SHOOTING = 0.5f;
    public const float FRAMETIME_PLAYER_DASH = 0.018f;
    public const float TOTAL_TIME_PLAYER_DASH = 0.1f;
    public const float TIME_PLAYER_NEW_DASH = 0.5f;
    public const float TIME_PLAYER_COMPLET_CHARGE = 1.3f;

    public static Vector2 PLAYER_SHOOT_DIFERENCE = new Vector2(0, 0.03f);

    public static float PLAYER_ACTION_POINTS_BY_TIME = 15;
    public static float PLAYER_MAX_ACTION_POINTS = 100;

    // Beta Variables
    public const float TIME_BETA_DISAPPEAR = 0.3f;
    public const float BETA_WAIT_APPEAR = 1f;

    // Enemies Variables
    public const float EXTRA_DASH_TIME = 0.07f;
    public const float DASH_SHADOW_TIME = 0.5f;
}