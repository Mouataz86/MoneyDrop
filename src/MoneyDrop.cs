using GTA;
using GTA.Native;
using GTA.Math;
using LemonUI;
using LemonUI.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class MoneyDrop : Script
{
    private ObjectPool pool = new ObjectPool();
    private NativeMenu menu;

    private NativeCheckboxItem toggleMod;
    private NativeListItem<string> maxBagsItem;
    private NativeListItem<string> moneyAmountItem;
    private NativeItem loadButton;
    private NativeItem saveButton;

    private List<Prop> activeBags = new List<Prop>();
    private string modelName = "prop_cash_case_01";
    private string iniPath = "scripts/MoneyDrop.ini";

    private Keys dropKey = Keys.F5;
    private Keys menuKey;

    private bool modEnabled = true;
    private int maxBags = 3;
    private int moneyAmount = 2500;
    private float pickupRange = 1.5f;
    private float maxDistance = 10.0f;

    public MoneyDrop()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;

        LoadSettings();
        SetupMenu();
    }

    private void LoadSettings()
    {
        if (!File.Exists(iniPath)) return;

        var settings = ScriptSettings.Load(iniPath);
        dropKey = settings.GetValue("General", "DropKey", Keys.F5);
        modEnabled = settings.GetValue("General", "ModEnabled", true);
        maxBags = settings.GetValue("General", "MaxBags", 3);
        moneyAmount = settings.GetValue("General", "MoneyAmount", 2500);
        modelName = settings.GetValue("General", "ModelName", "prop_cash_case_01");
        menuKey = settings.GetValue("General", "MenuKey", Keys.F10);
    }

    private void SaveSettings()
    {
        var settings = ScriptSettings.Load(iniPath);
        settings.SetValue("General", "DropKey", dropKey);
        settings.SetValue("General", "ModEnabled", modEnabled);
        settings.SetValue("General", "MaxBags", maxBags);
        settings.SetValue("General", "MoneyAmount", moneyAmount);
        settings.SetValue("General", "ModelName", modelName);
        settings.SetValue("General", "MenuKey", menuKey);
        settings.Save();
    }

    private void OnTick(object sender, EventArgs e)
    {
        pool.Process();

        Ped player = Game.Player.Character;
        List<Prop> toRemove = new List<Prop>();

        foreach (var bag in activeBags)
        {
            if (bag == null || !bag.Exists())
            {
                toRemove.Add(bag);
                continue;
            }

            float distance = player.Position.DistanceTo(bag.Position);

            if (distance < pickupRange)
            {
                Game.Player.Money += moneyAmount;
                bag.Delete();
                toRemove.Add(bag);
            }
            else if (distance > maxDistance)
            {
                bag.Delete();
                toRemove.Add(bag);
                GTA.UI.Notification.Show("~o~Money Briefcase despawned: Player is too far");
            }
        }

        foreach (var bag in toRemove)
        {
            activeBags.Remove(bag);
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == dropKey && modEnabled)
        {
            SpawnMoneyBag();
        }

        if (e.KeyCode == menuKey)
        {
            menu.Visible = !menu.Visible;
        }
    }

    private void SpawnMoneyBag()
    {
        Ped player = Game.Player.Character;

        if (Function.Call<bool>(Hash.IS_CUTSCENE_PLAYING) || player.IsInVehicle() || player.IsRagdoll || player.IsFalling)
        {
            GTA.UI.Notification.Show("~r~Cannot drop while in vehicle, cutscene, falling, or ragdoll.");
            return;
        }

        if (activeBags.Count >= maxBags)
        {
            GTA.UI.Notification.Show("~r~Max briefcases reached!");
            return;
        }

        Vector3 forward = player.ForwardVector.Normalized * 2.0f;
        Vector3 spawnPos = player.Position + forward + new Vector3(0, 0, 3.0f);

        Model model = new Model(modelName);
        if (!model.IsInCdImage || !model.IsValid)
            return;

        model.Request();
        int timeout = Game.GameTime + 2000;
        while (!model.IsLoaded && Game.GameTime < timeout)
        {
            Script.Yield();
        }

        if (model.IsLoaded)
        {
            Prop bag = World.CreateProp(model, spawnPos, true, true);
            bag.HasGravity = true;
            activeBags.Add(bag);
        }

        model.MarkAsNoLongerNeeded();
    }

    private void SetupMenu()
    {
        menu = new NativeMenu("MoneyDrop Settings", "Configure Money Drop");

        // 1. Toggle
        toggleMod = new NativeCheckboxItem("Enable Mod", modEnabled);
        toggleMod.CheckboxChanged += (s, e) => modEnabled = toggleMod.Checked;

        // 2. Max Briefcases
        List<string> bagOptions = new List<string>();
        for (int i = 1; i <= 10; i++) bagOptions.Add(i.ToString());
        maxBagsItem = new NativeListItem<string>("Max Briefcases", bagOptions.ToArray());
        maxBagsItem.SelectedIndex = Math.Min(Math.Max(maxBags - 1, 0), 9);
        maxBagsItem.Activated += (s, i) => maxBags = maxBagsItem.SelectedIndex + 1;

        // 3. Money Amount
        List<string> moneyOptions = new List<string>();
        for (int i = 0; i <= 15; i++) moneyOptions.Add((2500 + i * 500).ToString());
        moneyAmountItem = new NativeListItem<string>("Money Amount", moneyOptions.ToArray());
        moneyAmountItem.SelectedIndex = Math.Min(Math.Max((moneyAmount - 2500) / 500, 0), 15);
        moneyAmountItem.Activated += (s, i) => moneyAmount = 2500 + (moneyAmountItem.SelectedIndex * 500);

        // 4. Load from INI
        loadButton = new NativeItem("Load from INI");
        loadButton.Activated += (s, i) =>
        {
            LoadSettings();
            toggleMod.Checked = modEnabled;
            maxBagsItem.SelectedIndex = Math.Min(Math.Max(maxBags - 1, 0), 9);
            moneyAmountItem.SelectedIndex = Math.Min(Math.Max((moneyAmount - 2500) / 500, 0), 15);
            GTA.UI.Notification.Show("~g~Settings loaded from INI");
        };

        // 5. Save to INI
        saveButton = new NativeItem("Save Settings");
        saveButton.Activated += (s, i) =>
        {
            SaveSettings();
            GTA.UI.Notification.Show("~b~Settings saved to INI");
        };

        menu.Add(toggleMod);
        menu.Add(maxBagsItem);
        menu.Add(moneyAmountItem);
        menu.Add(loadButton);
        menu.Add(saveButton);
        pool.Add(menu);
    }
}
