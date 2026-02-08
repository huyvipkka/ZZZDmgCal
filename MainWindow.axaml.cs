using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace ZZZDmgCal;


public partial class MainWindow : Window, INotifyPropertyChanged
{
    private double _nonCritDamage;
    private double _critDamage;
    private double _averageDamage;

    private double _baseDmg;
    private double _dmgBonusMuti;
    private double _critMuti;
    private double _critMutiAve;
    private double _effectiveDef;
    private double _defMuti;
    private double _resMuti;
    private double _dmgTakenMuti;
    private double _StunMuti;


    public double NonCritDamage
    {
        get => _nonCritDamage;
        set => SetField(ref _nonCritDamage, value);
    }
    public double CritDamage
    {
        get => _critDamage;
        set => SetField(ref _critDamage, value);
    }
    public double AverageDamage
    {
        get => _averageDamage;
        set => SetField(ref _averageDamage, value);
    }

    public double BaseDmg
    {
        get => _baseDmg;
        set => SetField(ref _baseDmg, value);
    }
    public double DmgBonusMuti
    {
        get => _dmgBonusMuti;
        set => SetField(ref _dmgBonusMuti, value);
    }

    public double CritMuti
    {
        get => _critMuti;
        set => SetField(ref _critMuti, value);
    }
    public double CritMutiAve
    {
        get => _critMutiAve;
        set => SetField(ref _critMutiAve, value);
    }
    public double EffectiveDef
    {
        get => _effectiveDef;
        set => SetField(ref _effectiveDef, value);
    }
    public double DefMuti
    {
        get => _defMuti;
        set => SetField(ref _defMuti, value);
    }
    public double ResMuti
    {
        get => _resMuti;
        set => SetField(ref _resMuti, value);
    }
    public double DmgTakenMuti
    {
        get => _dmgTakenMuti;
        set => SetField(ref _dmgTakenMuti, value);
    }
    public double StunMuti
    {
        get => _StunMuti;
        set => SetField(ref _StunMuti, value);
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        InitDefaults();
        HookAutoRecalculate();
        RecalculateAll();
        PropertyChanged += OnBaseAndMutiChanged;
        RecalculateDamage();
    }


    private void InitDefaults()
    {
        // ===== Base DMG =====
        AtkTextBox.Text = "3000";
        ScaleTextBox.Text = "300";

        // ===== DMG Bonus =====
        AllDamageBonusTextBox.Text = "50";

        // ===== CRIT =====
        CritRateSlider.Value = 50;
        CritDmgTextBox.Text = "200";

        // ===== DEF =====
        EnemyDefSlider.Value = 800;
        DefReductionSlider.Value = 0;
        ArmorPenetrationSlider.Value = 0;
        ArmorPenFlatTextBox.Text = "0";
        // Level 10 → 60
        CharacterLevelComboBox.SelectedIndex = 5;

        // ===== RES =====
        EnemyResSlider.Value = 0;
        ResReductionSlider.Value = 0;
        ResIgnoreSlider.Value = 0;
        // ===== DMG Taken =====
        DmgTakenIncreaseSlider.Value = 0;
        DmgTakenReductionSlider.Value = 0;

        // ===== Stunned =====
        StunnedMultiplierSlider.Value = 1.5;

    }

    private void HookAutoRecalculate()
    {
        AtkTextBox.TextChanged += (_, __) => CalBaseDmg();
        ScaleTextBox.TextChanged += (_, __) => CalBaseDmg();

        AllDamageBonusTextBox.TextChanged += (_, __) => CalDmgBonusMuti();

        CritRateSlider.ValueChanged += (_, __) => CalCritMuti();
        CritDmgTextBox.TextChanged += (_, __) => CalCritMuti();

        EnemyDefSlider.ValueChanged += (_, __) => CalDefEffective();
        DefReductionSlider.ValueChanged += (_, __) => CalDefEffective();
        ArmorPenetrationSlider.ValueChanged += (_, __) => CalDefEffective();
        ArmorPenFlatTextBox.TextChanged += (_, __) => CalDefEffective();
        CharacterLevelComboBox.SelectionChanged += (_, __) => CalDefMuti();

        EnemyResSlider.ValueChanged += (_, __) => CalResMuti();
        ResReductionSlider.ValueChanged += (_, __) => CalResMuti();
        ResIgnoreSlider.ValueChanged += (_, __) => CalResMuti();

        DmgTakenIncreaseSlider.ValueChanged += (_, __) => CalDmgTakenMuti();
        DmgTakenReductionSlider.ValueChanged += (_, __) => CalDmgTakenMuti();

        StunnedMultiplierSlider.ValueChanged += (_, __) => CalStunMuti();
    }

    private void CalBaseDmg()
    {
        double atk = Parse(AtkTextBox.Text);
        double scale = Parse(ScaleTextBox.Text) / 100.0;
        BaseDmg = atk * scale;
    }

    private void CalDmgBonusMuti()
    {
        double dmgBonus = 1 + Parse(AllDamageBonusTextBox.Text) / 100.0;
        DmgBonusMuti = dmgBonus;
    }

    private void CalCritMuti()
    {
        double critRate = CritRateSlider.Value / 100.0;
        double critDmg = Parse(CritDmgTextBox.Text) / 100.0;
        CritMuti = 1 + critDmg;
        CritMutiAve = 1 + critRate * critDmg;
    }
    private void CalDefEffective()
    {
        // Enemy Def
        double enemyDef = EnemyDefSlider.Value;
        double defReduction = DefReductionSlider.Value / 100.0;
        double armorPen = ArmorPenetrationSlider.Value / 100.0;
        double armorPenFlat = Parse(ArmorPenFlatTextBox.Text);
        EffectiveDef = Math.Max(enemyDef * (1 - defReduction) * (1 - armorPen) - armorPenFlat, 0);
        CalDefMuti();
    }
    private void CalDefMuti()
    {

        int characterLevel = 60;
        if (CharacterLevelComboBox.SelectedItem is ComboBoxItem item)
        {
            characterLevel = int.Parse(item.Content!.ToString()!);
        }
        int levelFactor = GetLevelFactor(characterLevel);
        DefMuti = levelFactor / (Math.Max(EffectiveDef, 0) + levelFactor);
    }

    private void CalResMuti()
    {
        double enemyRes = EnemyResSlider.Value / 100.0;
        double resRedu = ResReductionSlider.Value / 100.0;
        double resIgnore = ResIgnoreSlider.Value / 100.0;

        ResMuti = 1 - enemyRes + resRedu + resIgnore;
    }

    private void CalDmgTakenMuti()
    {
        double dmgTakenIncrease = DmgTakenIncreaseSlider.Value / 100.0;
        double dmgTakenReduction = DmgTakenReductionSlider.Value / 100.0;
        DmgTakenMuti = 1 + dmgTakenIncrease - dmgTakenReduction;
    }

    private void CalStunMuti()
    {
        StunMuti = StunnedMultiplierSlider.Value;
    }

    private void RecalculateDamage()
    {
        double finalBase =
            BaseDmg *
            DmgBonusMuti *
            DefMuti *
            ResMuti *
            DmgTakenMuti *
            StunMuti;

        NonCritDamage = finalBase;

        CritDamage = finalBase * CritMuti;

        AverageDamage = finalBase * CritMutiAve;
    }

    private void OnBaseAndMutiChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(BaseDmg):
            case nameof(DmgBonusMuti):
            case nameof(CritMuti):
            case nameof(CritMutiAve):
            case nameof(DefMuti):
            case nameof(ResMuti):
            case nameof(DmgTakenMuti):
            case nameof(StunMuti):
                RecalculateDamage();
                break;
        }
    }


    private int GetLevelFactor(int level)
    {
        return level switch
        {
            10 => 94,
            20 => 172,
            30 => 281,
            40 => 421,
            50 => 592,
            60 => 794,
            _ => 794
        };
    }

    private void RecalculateAll()
    {
        CalBaseDmg();
        CalDmgBonusMuti();
        CalCritMuti();
        CalDefEffective();
        CalResMuti();
        CalDmgTakenMuti();
        CalStunMuti();

        // CalDefEffective đã gọi CalDefMuti
        // Damage sẽ auto tính qua PropertyChanged
    }

    private double Parse(string? s)
        => double.TryParse(s, out var v) ? v : 0;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (!Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }



	private void OpenSettings(object? sender, RoutedEventArgs e)
	{
        var win = new SettingsWindow();
		win.ShowDialog(this);
	}

}