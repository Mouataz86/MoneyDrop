using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

public class MoneyDrop : Script
{
    private Keys dropKey = Keys.F5;
    private int moneyAmount = 2500;
    private string modelName = "prop_cash_case_01";
    private List<Prop> activeBags = new List<Prop>();
    private string iniPath = "scripts/MoneyDrop.ini";

    public MoneyDrop()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (File.Exists(iniPath))
        {
            ScriptSettings settings = ScriptSettings.Load(iniPath);
            dropKey = settings.GetValue("General", "DropKey", Keys.F5);
            moneyAmount = settings.GetValue("General", "MoneyAmount", 2500);
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == dropKey)
        {
            SpawnMoneyBag();
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        Ped player = Game.Player.Character;
        List<Prop> collectedBags = new List<Prop>();

        foreach (var bag in activeBags)
        {
            if (bag != null && bag.Exists())
            {
                float distance = player.Position.DistanceTo(bag.Position);
                if (distance < 1.5f)
                {
                    Game.Player.Money += moneyAmount;
                    bag.Delete();
                    collectedBags.Add(bag);
                }
            }
            else
            {
                collectedBags.Add(bag); // remove invalid/deleted bags
            }
        }

        // Remove collected or deleted bags from the active list
        foreach (var bag in collectedBags)
        {
            activeBags.Remove(bag);
        }
    }

    private void SpawnMoneyBag()
    {
        Ped player = Game.Player.Character;
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
    }
}
