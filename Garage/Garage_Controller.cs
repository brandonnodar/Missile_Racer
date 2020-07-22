using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garage_Controller : MonoBehaviour
{
    public GameObject camPos;

    [Space]
    [Header("-----/// VARIABLES ///-----")]
    public Vector2 freeroamSpawnPos;

    [Space]
    [Header("-----/// CLASSES ///-----")]
    public Garage_UI ui;
    public Garage_AutoLander autoLander;
    public Garage_ItemEffects itemEffects;
    public Garage_SFX sfx;
    public Camera_Manager cameraManager;
    public Player_Manager playerManager;
    public Missile_MomentumDirection playerMissileMomentum;
    public SpawnController spawnController;
    public Camera_Follow cameraFollow;
    public Camera_Zoom cameraZoom;
    public ControllerBindings controllerBindings;
    public GameSave_Controller gameSaveController;
    public PlayerHUDInfo playerHUDInfo;

    [Space]
    [Header("-----/// SCRIPTABLE OBJECTS ///-----")]
    public SO_GameSaveData gameSaveData;
    public SO_ItemsInventory itemsInventory;

    int missileIndexSelected = 0;
    int performanceIndexSelected = 0;
    int thrusterIndexSelected = 0;
    int explosionIndexSelected = 0;
    bool playerIsInGarage;

    private void Awake()
    {
        ui.SetEnterButtonState(false);
        playerIsInGarage = false;
        itemEffects.StopThruster();
        itemEffects.StopExplosion();

        SetItemUnlockStats();
    }

    private void Update()
    {
        if (playerIsInGarage)
        {
            if (controllerBindings.characterActions.goBackUI.WasPressed)
            {
                BackButtonPressed();
            }

            if (controllerBindings.characterActions.escapeUIPanel.WasPressed)
            {
                ExitGarage();
            }
        }
    }

    public void EnterGarage()
    {
        spawnController.SetRespawnPos(freeroamSpawnPos);
        spawnController.SaveRespawnPos();
        autoLander.LandPlayerMissile();
        ui.SetEnterButtonState(false);
        ShowMainMenu();

        cameraManager.ZoomInGarage(camPos);

        sfx.PlayMissileAutoLander();
        sfx.PlayMissileThrusters();
        sfx.PlayEnterGarage();

        playerIsInGarage = true;
    }

    public void ExitGarage()
    {
        playerIsInGarage = false;
        ui.SetEnterButtonState(true);
        ui.SetMainFocusButton("ENTERGARAGE");
        ui.HideAllSwapPanels();
        itemEffects.StopThruster();
        itemEffects.StopExplosion();
        ui.SetMainPanelState(false);
        cameraManager.ExitGarageZoom();
        autoLander.AllowMissileFlying();

        cameraFollow.CalculateSmoothTime();
        cameraZoom.CalculateZoomRange();
        playerMissileMomentum.UpdatePointScalar();

        sfx.PlayMainPanelHide();
    }

    public void BackButtonPressed()
    {
        ui.BackButtonPressed();
        sfx.PlayBackButton();
    }

    public void ShowMainMenu()
    {
        ui.HideAllSwapPanels();
        ui.SetMainPanelState(true);
        ui.SetSwapPanelState("MAINMENU", true);
        ui.SetMainFocusButton("MAINMENU");
        sfx.PlayMainPanelShow();
    }

    void SetItemUnlockStats()
    {
        int missilesUnlocked = 0;
        int maxMissiles = gameSaveData.missileBodies.bodies.Length;
        int performancesUnlocked = 0;
        int maxPerformances = 0;
        int bodyColorsUnlocked = 0;
        int maxBodyColors = 0;
        for (int i = 0; i < maxMissiles; i++)
        {
            if (gameSaveData.missileBodies.bodies[i].isBodyUnlocked)
                missilesUnlocked++;

            for (int n = 0; n < gameSaveData.missileBodies.bodies[i].isPerformanceUnlocked.Length; n++)
            {
                maxPerformances++;
                if (gameSaveData.missileBodies.bodies[i].isPerformanceUnlocked[n])
                    performancesUnlocked++;
            }

            for (int a = 0; a < gameSaveData.missileBodies.bodies[i].isBodyColorUnlocked.Length; a++)
            {
                maxBodyColors++;
                if (gameSaveData.missileBodies.bodies[i].isBodyColorUnlocked[a])
                    bodyColorsUnlocked++;
            }
        }

        int thrustersUnlocked = 0;
        int maxThrusters = gameSaveData.thrusterItemsUnlocked.isUnlocked.Length;
        for (int i = 0; i < maxThrusters; i++)
        {
            if (gameSaveData.thrusterItemsUnlocked.isUnlocked[i])
                thrustersUnlocked++;
        }

        int explosionsUnlocked = 0;
        int maxExplosions = gameSaveData.explosionItemsUnlocked.isUnlocked.Length;
        for (int i = 0; i < maxExplosions; i++)
        {
            if (gameSaveData.explosionItemsUnlocked.isUnlocked[i])
                explosionsUnlocked++;
        }

        ui.UpdateUnlockStats("MISSILES", missilesUnlocked, maxMissiles);
        ui.UpdateUnlockStats("PERFORMANCE", performancesUnlocked, maxPerformances);
        ui.UpdateUnlockStats("BODYCOLORS", bodyColorsUnlocked, maxBodyColors);
        ui.UpdateUnlockStats("THRUSTERS", thrustersUnlocked, maxThrusters);
        ui.UpdateUnlockStats("EXPLOSIONS", explosionsUnlocked, maxExplosions);
    }
    #region MISSILES
    public void ShowMissilesPanel()
    {
        ui.SetSwapPanelState("MAINMENU", false);
        ui.SetSwapPanelState("MISSILES", true);

        ui.ScrollContentToUse("MISSILES");

        ui.MissileSelected(gameSaveData.playerMissileSettings.missileBodyIndex);
        ui.MissileEquipped(gameSaveData.playerMissileSettings.missileBodyIndex);
        ui.SetMainFocusButton("MISSILES", gameSaveData.playerMissileSettings.missileBodyIndex);

        sfx.PlayOptionButton();

        // Set each missile box's settings in the Missiles Panel.
        for (int i = 0; i < gameSaveData.missileBodies.bodies.Length; i++)
        {
            ui.SetMissilePanelUnlockColor(i, gameSaveData.missileBodies.bodies[i].isBodyUnlocked);
            ui.SetMissileBodyRatingColor(i, gameSaveData.missileBodies.bodies[i].isBodyUnlocked);

            // Update the performance lights for each missile (should show unlocked performances).
            int numOfPerformancesUnlocked = 0;
            for (int n = 0; n < gameSaveData.missileBodies.bodies[i].isPerformanceUnlocked.Length; n++)
            {
                if (gameSaveData.missileBodies.bodies[i].isPerformanceUnlocked[n])
                    numOfPerformancesUnlocked++;
            }
            ui.SetMissilePLights(i, numOfPerformancesUnlocked);

            // Set missile preset info.
            ui.SetMissilePresetInfo(i, gameSaveData.garageMissilePresets.bodyColorIndexs[i],
                gameSaveData.garageMissilePresets.performanceIndexs[i], gameSaveData.garageMissilePresets.thrusterIndexs[i],
                gameSaveData.garageMissilePresets.explosionIndexs[i]);  
        }
    }

    public void MissileSelected(int itemIndex)
    {
        missileIndexSelected = itemIndex;
        ui.MissileSelected(itemIndex);
    }

    public void MissileEquipped(int itemIndex)
    {
        if (gameSaveData.missileBodies.bodies[itemIndex].isBodyUnlocked)
        {
            ui.MissileEquipped(itemIndex);

            int bodyColorIndex = gameSaveData.garageMissilePresets.bodyColorIndexs[itemIndex];
            gameSaveData.playerMissileSettings.bodyColorIndex = bodyColorIndex;
            gameSaveData.playerMissileSettings.missileBodyIndex = itemIndex;
            gameSaveData.playerMissileSettings.missileClass = itemsInventory.missileBodies[itemIndex].className.ToString();
            gameSaveData.playerMissileSettings.missileBodyName = itemsInventory.missileBodies[itemIndex].modelName;
            playerManager.missileBody.SetMissileBody();

            sfx.PlayItemEquipped();

            int performanceIndex = gameSaveData.garageMissilePresets.performanceIndexs[itemIndex];
            float rotation = itemsInventory.missileBodies[itemIndex].playerRotationSpeed[performanceIndex];
            float maxSpeed = itemsInventory.missileBodies[itemIndex].maxSpeed[performanceIndex];
            float thrust = itemsInventory.missileBodies[itemIndex].thrustPower[performanceIndex];
            gameSaveData.playerMissileSettings.freeroamPerformanceIndex = performanceIndex;
            gameSaveData.playerMissileSettings.freeroamRotSpeed = rotation;
            gameSaveData.playerMissileSettings.freeroamMaxSpeed = maxSpeed;
            gameSaveData.playerMissileSettings.freeroamThrustPower = thrust;
            playerManager.flying.SetPerformance();

            // Thruster
            int thrusterIndex = gameSaveData.garageMissilePresets.thrusterIndexs[itemIndex];
            gameSaveData.playerMissileSettings.thrusterIndex = thrusterIndex;

            playerManager.thrusters.SetThrusterCosmetics(itemsInventory.thrusterItems[thrusterIndex].thrusterMaterial);
            sfx.PlayItemEquipped();

            // Explosion
            int explosionIndex = gameSaveData.garageMissilePresets.explosionIndexs[itemIndex];
            gameSaveData.playerMissileSettings.explosionIndex = explosionIndex;

            playerManager.explosion.SetExplosionCosmetic();
            sfx.PlayItemEquipped();

            gameSaveController.SavePlayerMissileSettingsData();
            playerHUDInfo.SetHUDInfo();
        }
        else
        {
            sfx.PlayDenied();
        }
    }
    #endregion MISSILES

    public void ShowUpgradesPanel()
    {
        ui.SetSwapPanelState("MAINMENU", false);
        ui.SetSwapPanelState("UPGRADE", true);
        ui.SetMainFocusButton("UPGRADES");

        sfx.PlayOptionButton();
    }

    #region BODY COLORS
    public void ShowBodyColorsPanel()
    {
        ui.SetSwapPanelState("UPGRADE", false);
        ui.SetSwapPanelState("BODYCOLORS", true);
        ui.BodyColorItemEquipped(gameSaveData.playerMissileSettings.bodyColorIndex);
        ui.SetMainFocusButton("BODYCOLORS", gameSaveData.playerMissileSettings.bodyColorIndex);

        sfx.PlayOptionButton();

        int playerMissileIndex = gameSaveData.playerMissileSettings.missileBodyIndex;
        for (int i = 0; i < gameSaveData.missileBodies.bodies[playerMissileIndex].isBodyColorUnlocked.Length; i++)
        {
            ui.SetBodyColorInfo(playerMissileIndex, i, gameSaveData.missileBodies.bodies[playerMissileIndex].isBodyColorUnlocked[i]);
        }
    }

    public void EquipBodyColor(int itemIndex)
    {
        if (gameSaveData.missileBodies.bodies[gameSaveData.playerMissileSettings.missileBodyIndex].isBodyColorUnlocked[itemIndex])
        {
            ui.BodyColorItemEquipped(itemIndex);
            gameSaveData.playerMissileSettings.bodyColorIndex = itemIndex;
            gameSaveData.garageMissilePresets.bodyColorIndexs[gameSaveData.playerMissileSettings.missileBodyIndex] = itemIndex;
            playerManager.missileBody.SetMissileBody();

            gameSaveController.SavePlayerMissileSettingsData();
            gameSaveController.SaveGarageMissilePresetsData();
            sfx.PlayItemEquipped();
        } else
        {
            sfx.PlayDenied();
        }
    }
    #endregion BODY COLORS

    #region PERFORMANCE
    public void ShowPerformancePanel()
    {
        int missileIndex = gameSaveData.playerMissileSettings.missileBodyIndex;
        for (int i = 0; i < itemsInventory.missileBodies[missileIndex].performanceRatingRequired.Length; i++)
        {
            ui.SetPerformanceRating(itemsInventory.missileBodies[missileIndex].performanceRatingRequired[i], i, gameSaveData.missileBodies.bodies[missileIndex].isPerformanceUnlocked[i]);
        }

        ui.SetSwapPanelState("UPGRADE", false);
        ui.SetSwapPanelState("PERFORMANCE", true);
        ui.SetMainFocusButton("PERFORMANCE", gameSaveData.playerMissileSettings.freeroamPerformanceIndex);

        ui.PerformanceItemEquipped(gameSaveData.playerMissileSettings.freeroamPerformanceIndex);

        sfx.PlayOptionButton();
    }

    public void PerformanceItemSelected(int performanceIndex)
    {
        performanceIndexSelected = performanceIndex;
    }

    public void EquipPerformance(int performanceIndex)
    {
        if (gameSaveData.missileBodies.bodies[gameSaveData.playerMissileSettings.missileBodyIndex].isPerformanceUnlocked[performanceIndex])
        {
            ui.PerformanceItemEquipped(performanceIndex);

            int missileIndex = gameSaveData.playerMissileSettings.missileBodyIndex;
            float rotation = itemsInventory.missileBodies[missileIndex].playerRotationSpeed[performanceIndex];
            float maxSpeed = itemsInventory.missileBodies[missileIndex].maxSpeed[performanceIndex];
            float thrust = itemsInventory.missileBodies[missileIndex].thrustPower[performanceIndex];
            gameSaveData.playerMissileSettings.freeroamPerformanceIndex = performanceIndex;
            gameSaveData.playerMissileSettings.freeroamRotSpeed = rotation;
            gameSaveData.playerMissileSettings.freeroamMaxSpeed = maxSpeed;
            gameSaveData.playerMissileSettings.freeroamThrustPower = thrust;
            playerManager.flying.SetPerformance();

            gameSaveData.garageMissilePresets.performanceIndexs[missileIndex] = performanceIndex;

            gameSaveController.SavePlayerMissileSettingsData();
            gameSaveController.SaveGarageMissilePresetsData();

            playerHUDInfo.SetHUDInfo();
            sfx.PlayItemEquipped();
        } else
        {
            sfx.PlayDenied();
        }
    }
    #endregion PERFORMANCE


    public void ShowThrustersPanel()
    {
        ui.ScrollContentToUse("THRUSTERS");
        ui.SetSwapPanelState("UPGRADE", false);
        ui.SetSwapPanelState("THRUSTERS", true);

        // Auto select the player's currently equipped thruster.
        ui.SetMainFocusButton("THRUSTERS", gameSaveData.playerMissileSettings.thrusterIndex);
        ui.ThrusterItemSelected(gameSaveData.playerMissileSettings.thrusterIndex);
        ui.ThrusterItemEquipped(gameSaveData.playerMissileSettings.thrusterIndex);

        sfx.PlayOptionButton();

        for (int i = 0; i < gameSaveData.thrusterItemsUnlocked.isUnlocked.Length; i++)
        {
            ui.SetThrusterItemVisible(i, gameSaveData.thrusterItemsUnlocked.isUnlocked[i]);
        }
    }

    public void ThrusterItemSelected(int itemIndex)
    {
        thrusterIndexSelected = itemIndex;
        ui.ThrusterItemSelected(itemIndex);
        itemEffects.PlayThruster(itemsInventory.thrusterItems[itemIndex].thrusterMaterial);
    }

    public void ThrusterItemEquipped(int itemIndex)
    {
        gameSaveData.playerMissileSettings.thrusterIndex = itemIndex;
        gameSaveController.SavePlayerMissileSettingsData();

        gameSaveData.garageMissilePresets.thrusterIndexs[gameSaveData.playerMissileSettings.missileBodyIndex] = itemIndex;
        gameSaveController.SaveGarageMissilePresetsData();

        playerManager.thrusters.SetThrusterCosmetics(itemsInventory.thrusterItems[itemIndex].thrusterMaterial);
        ui.ThrusterItemEquipped(itemIndex);
        sfx.PlayItemEquipped();
    }

    public void ShowExplosionsPanel()
    {
        ui.ScrollContentToUse("EXPLOSIONS");
        ui.SetSwapPanelState("UPGRADE", false);
        ui.SetSwapPanelState("EXPLOSIONS", true);

        // Auto select the player's currently equipped thruster.
        ui.SetMainFocusButton("EXPLOSIONS", gameSaveData.playerMissileSettings.explosionIndex);
        ui.ExplosionItemSelected(gameSaveData.playerMissileSettings.explosionIndex);
        ui.ExplosionItemEquipped(gameSaveData.playerMissileSettings.explosionIndex);

        sfx.PlayOptionButton();

        for (int i = 0; i < gameSaveData.explosionItemsUnlocked.isUnlocked.Length; i++)
        {
            ui.SetExplosionItemVisible(i, gameSaveData.explosionItemsUnlocked.isUnlocked[i]);
        }
    }

    public void ExplosionItemSelected(int itemIndex)
    {
        explosionIndexSelected = itemIndex;
        ui.ExplosionItemSelected(itemIndex);
        itemEffects.PlayExplosion(itemsInventory.explosionItems[itemIndex].explosionMaterial);
    }

    public void ExplosionItemEquipped(int itemIndex)
    {
        gameSaveData.playerMissileSettings.explosionIndex = itemIndex;
        gameSaveController.SavePlayerMissileSettingsData();

        gameSaveData.garageMissilePresets.explosionIndexs[gameSaveData.playerMissileSettings.missileBodyIndex] = itemIndex;
        gameSaveController.SaveGarageMissilePresetsData();

        playerManager.explosion.SetExplosionCosmetic();
        ui.ExplosionItemEquipped(itemIndex);
        sfx.PlayItemEquipped();
    }
}
