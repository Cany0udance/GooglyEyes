using Godot;

namespace GooglyEyes;

public static class CardGooglyEyesRegistry
{
    /// <summary>
    /// Keyed by CardModel.Id.Entry (e.g. "STRIKE_RED", "DEFEND_GREEN").
    /// Populated by the editor's export, or by hand.
    /// </summary>
    public static readonly Dictionary<string, CardEyeConfig[]> Configs = new()
    {
        { "ABRASIVE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-117.9f, -304.3f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-162.3f, -303.2f), Scale = 0.20f },
        }},
        { "ACCELERANT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-182.8f, -318.0f), Scale = 0.11f },
        }},
        { "ACROBATICS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-187.4f, -306.3f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-198.4f, -306.0f), Scale = 0.03f },
        }},
        { "AFTERIMAGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-218.3f, -328.7f), Scale = 0.08f },
        }},
        { "ALL_FOR_ONE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-86.9f, -305.0f), Scale = 0.15f },
            new CardEyeConfig { Offset = new Vector2(-105.6f, -262.4f), Scale = 0.31f },
        }},
        { "ANGER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-98.3f, -319.6f), Scale = 0.10f },
        }},
        { "APOTHEOSIS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-150.3f, -341.7f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-154.8f, -341.8f), Scale = 0.02f },
        }},
        { "APPARITION", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-178.9f, -290.3f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-127.8f, -290.5f), Scale = 0.20f },
        }},
        { "ARSENAL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-64.3f, -260.3f), Scale = 0.03f },
        }},
        { "ASTRAL_PULSE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-167.3f, -324.1f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-151.9f, -329.2f), Scale = 0.07f },
        }},
        { "BAD_LUCK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-101.8f, -263.8f), Scale = 0.09f },
        }},
        { "BANSHEES_CRY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-207.5f, -304.3f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-190.1f, -303.1f), Scale = 0.07f },
        }},
        { "BARRAGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-177.2f, -329.0f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-63.8f, -265.7f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-70.6f, -265.8f), Scale = 0.02f },
        }},
        { "BEACON_OF_HOPE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-155.1f, -278.0f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-242.3f, -306.9f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-235.2f, -303.4f), Scale = 0.03f },
        }},
        { "BEAT_INTO_SHAPE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-142.5f, -323.1f), Scale = 0.20f },
        }},
        { "BECKON", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-152.8f, -289.3f), Scale = 0.80f },
        }},
        { "BEGONE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-228.4f, -303.9f), Scale = 0.08f },
        }},
        { "BELIEVE_IN_YOU", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-88.3f, -257.5f), Scale = 0.15f },
        }},
        { "BLACK_HOLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-184.2f, -346.6f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-150.0f, -355.7f), Scale = 0.10f },
        }},
        { "BLADE_DANCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-170.1f, -346.6f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-165.0f, -347.8f), Scale = 0.02f },
        }},
        { "BLUR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-236.6f, -312.5f), Scale = 0.04f },
        }},
        { "BODY_SLAM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-180.7f, -350.6f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-171.6f, -353.3f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-83.8f, -341.6f), Scale = 0.05f },
        }},
        { "BODYGUARD", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-251.6f, -245.9f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-242.9f, -246.7f), Scale = 0.04f },
        }},
        { "BOOST_AWAY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-139.3f, -252.6f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-221.1f, -286.8f), Scale = 0.07f },
        }},
        { "BOOT_SEQUENCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-176.3f, -321.8f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-166.2f, -304.3f), Scale = 0.10f },
        }},
        { "BOUNCING_FLASK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-118.4f, -271.3f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-129.3f, -278.3f), Scale = 0.04f },
        }},
        { "BRAND", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-189.0f, -278.6f), Scale = 0.17f },
            new CardEyeConfig { Offset = new Vector2(-152.6f, -290.8f), Scale = 0.17f },
        }},
        { "BREAK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-168.2f, -353.4f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-185.8f, -344.5f), Scale = 0.06f },
        }},
        { "BREAKTHROUGH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-158.3f, -301.3f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-176.8f, -296.2f), Scale = 0.05f },
        }},
        { "BULLET_TIME", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-164.6f, -313.4f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-152.1f, -312.3f), Scale = 0.05f },
        }},
        { "BULLY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-127.5f, -296.1f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-76.4f, -265.0f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-91.2f, -267.4f), Scale = 0.06f },
        }},
        { "BUNDLE_OF_JOY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-191.5f, -291.8f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-129.5f, -330.3f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-73.8f, -321.8f), Scale = 0.20f },
        }},
        { "BURST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-198.0f, -283.3f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-241.7f, -282.0f), Scale = 0.09f, Opacity = 0.40f },
        }},
        { "BURY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-135.1f, -267.7f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-118.6f, -266.6f), Scale = 0.06f },
        }},
        { "BYRD_SWOOP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-138.0f, -337.4f), Scale = 0.08f },
        }},
        { "CALAMITY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-143.8f, -277.5f), Scale = 0.30f },
        }},
        { "CALL_OF_THE_VOID", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-154.4f, -268.9f), Scale = 0.01f },
            new CardEyeConfig { Offset = new Vector2(-151.6f, -268.9f), Scale = 0.01f },
        }},
        { "CALTROPS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-163.8f, -356.2f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-168.3f, -355.8f), Scale = 0.02f },
        }},
        { "CAPTURE_SPIRIT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-72.3f, -276.1f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-215.3f, -217.5f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-192.3f, -220.3f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-65.0f, -276.2f), Scale = 0.02f },
        }},
        { "CELESTIAL_MIGHT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-148.7f, -315.3f), Scale = 0.18f },
        }},
        { "CHARGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-151.0f, -284.5f), Scale = 0.50f },
        }},
        { "CHILL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-161.1f, -253.5f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-136.9f, -252.9f), Scale = 0.09f },
        }},
        { "CINDER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-117.7f, -347.5f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-133.4f, -350.9f), Scale = 0.05f },
        }},
        { "CLEANSE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-171.6f, -334.6f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-156.5f, -337.5f), Scale = 0.05f },
        }},
        { "CLOAK_OF_STARS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-149.9f, -347.5f), Scale = 0.06f },
        }},
        { "COLLISION_COURSE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-85.2f, -294.2f), Scale = 0.11f },
        }},
        { "COLOSSUS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-149.6f, -237.9f), Scale = 0.17f },
            new CardEyeConfig { Offset = new Vector2(-98.3f, -357.9f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-103.5f, -356.1f), Scale = 0.02f },
        }},
        { "COMET", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-145.6f, -247.4f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-172.1f, -276.0f), Scale = 0.10f },
        }},
        { "COMPILE_DRIVER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-224.2f, -266.2f), Scale = 0.06f },
        }},
        { "CONQUEROR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-116.7f, -345.6f), Scale = 0.08f },
        }},
        { "CONVERGENCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-103.1f, -302.4f), Scale = 0.15f },
        }},
        { "COOLHEADED", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-148.1f, -235.2f), Scale = 0.18f },
        }},
        { "COORDINATE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-160.6f, -314.4f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-188.9f, -308.5f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-182.5f, -331.0f), Scale = 0.07f },
        }},
        { "CORROSIVE_WAVE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-223.9f, -332.9f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-180.1f, -235.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-85.1f, -252.4f), Scale = 0.11f },
        }},
        { "CORRUPTION", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-209.6f, -296.8f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-241.3f, -286.6f), Scale = 0.10f },
        }},
        { "COSMIC_INDIFFERENCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-118.0f, -312.8f), Scale = 0.10f },
        }},
        { "COUNTDOWN", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.8f, -327.1f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-142.4f, -326.7f), Scale = 0.06f },
        }},
        { "CRASH_LANDING", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-155.9f, -328.2f), Scale = 0.08f },
        }},
        { "CRIMSON_MANTLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-58.5f, -318.9f), Scale = 0.10f },
        }},
        { "CRUELTY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-194.7f, -274.8f), Scale = 0.20f },
        }},
        { "CRUSH_UNDER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-134.8f, -309.4f), Scale = 0.04f },
        }},
        { "CURSE_OF_THE_BELL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-179.0f, -325.7f), Scale = 0.14f },
            new CardEyeConfig { Offset = new Vector2(-206.1f, -309.4f), Scale = 0.12f },
        }},
        { "DAGGER_SPRAY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-83.0f, -218.1f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-58.9f, -224.6f), Scale = 0.07f },
        }},
        { "DANSE_MACABRE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-122.9f, -283.3f), Scale = 0.04f },
        }},
        { "DARK_EMBRACE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-191.5f, -326.4f), Scale = 0.15f },
        }},
        { "DARK_SHACKLES", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-98.0f, -354.7f), Scale = 0.10f },
        }},
        { "DEADLY_POISON", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-161.6f, -319.9f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-132.5f, -319.5f), Scale = 0.11f },
        }},
        { "DEATH_MARCH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-145.9f, -292.5f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-163.0f, -292.5f), Scale = 0.06f },
        }},
        { "DEATHBRINGER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-126.0f, -338.3f), Scale = 0.01f },
            new CardEyeConfig { Offset = new Vector2(-128.9f, -338.4f), Scale = 0.01f },
        }},
        { "DEATHS_DOOR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-57.5f, -311.4f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-168.1f, -290.0f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-158.2f, -285.5f), Scale = 0.04f },
        }},
        { "DEBT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-84.1f, -291.6f), Scale = 0.12f },
        }},
        { "DECAY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-172.3f, -319.4f), Scale = 0.04f },
        }},
        { "DECISIONS_DECISIONS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-145.1f, -331.9f), Scale = 0.10f },
        }},
        { "DEFEND_DEFECT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-83.4f, -318.0f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-97.9f, -289.3f), Scale = 0.21f },
        }},
        { "DEFEND_NECROBINDER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-142.1f, -307.0f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-165.5f, -294.8f), Scale = 0.08f },
        }},
        { "DEFEND_REGENT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-239.5f, -263.4f), Scale = 0.08f },
        }},
        { "DEFEND_SILENT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-135.6f, -330.6f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-108.6f, -333.7f), Scale = 0.08f },
        }},
        { "DEFILE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-222.1f, -248.6f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-228.5f, -266.0f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-214.3f, -277.1f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-225.7f, -293.3f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-200.6f, -300.8f), Scale = 0.08f },
        }},
        { "DEFY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-120.2f, -312.0f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-146.2f, -308.7f), Scale = 0.06f },
        }},
        { "DELAY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-173.2f, -299.7f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-160.5f, -311.3f), Scale = 0.06f },
        }},
        { "DEMON_FORM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-140.7f, -289.7f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-156.3f, -290.4f), Scale = 0.06f },
        }},
        { "DEMONIC_SHIELD", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-121.0f, -340.7f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-79.8f, -337.3f), Scale = 0.15f },
            new CardEyeConfig { Offset = new Vector2(-220.9f, -316.4f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-180.7f, -277.4f), Scale = 0.04f },
        }},
        { "DEVASTATE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-241.7f, -357.1f), Scale = 0.01f },
            new CardEyeConfig { Offset = new Vector2(-145.3f, -322.3f), Scale = 0.06f },
        }},
        { "DEVOUR_LIFE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-156.4f, -276.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-169.9f, -272.7f), Scale = 0.03f },
        }},
        { "DISINTEGRATION", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-223.4f, -304.1f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-207.2f, -306.6f), Scale = 0.07f },
        }},
        { "DOMINATE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-119.2f, -308.6f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-142.4f, -306.5f), Scale = 0.08f },
        }},
        { "DOUBT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-114.9f, -356.3f), Scale = 0.12f },
        }},
        { "DRAIN_POWER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-133.5f, -255.5f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-194.7f, -258.9f), Scale = 0.15f },
        }},
        { "DRAMATIC_ENTRANCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-155.1f, -304.7f), Scale = 0.01f },
            new CardEyeConfig { Offset = new Vector2(-157.3f, -305.0f), Scale = 0.01f },
        }},
        { "DUAL_WIELD", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-110.0f, -344.5f), Scale = 0.06f },
        }},
        { "DUALCAST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-69.2f, -312.4f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-85.3f, -315.3f), Scale = 0.04f },
        }},
        { "ECHO_FORM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-152.8f, -300.6f), Scale = 0.12f },
            new CardEyeConfig { Offset = new Vector2(-153.5f, -324.5f), Scale = 0.05f },
        }},
        { "ECHOING_SLASH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-223.0f, -296.3f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-156.2f, -325.0f), Scale = 0.07f },
        }},
        { "ENERGY_SURGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-206.4f, -258.9f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-153.8f, -349.9f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-153.9f, -341.1f), Scale = 0.05f },
        }},
        { "ENLIGHTENMENT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-164.5f, -315.6f), Scale = 0.05f },
        }},
        { "ENVENOM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-141.8f, -296.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-155.5f, -296.9f), Scale = 0.05f },
        }},
        { "ERADICATE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-176.5f, -326.1f), Scale = 0.14f },
            new CardEyeConfig { Offset = new Vector2(-204.2f, -292.4f), Scale = 0.16f },
        }},
        { "EVIL_EYE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-181.9f, -301.6f), Scale = 0.30f },
            new CardEyeConfig { Offset = new Vector2(-56.7f, -256.2f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-76.6f, -256.6f), Scale = 0.05f },
        }},
        { "EXPOSE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-184.7f, -263.8f), Scale = 0.09f },
        }},
        { "EXTERMINATE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-169.0f, -250.0f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-160.6f, -253.1f), Scale = 0.03f },
        }},
        { "FEAR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-189.5f, -288.4f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-128.4f, -290.2f), Scale = 0.20f },
        }},
        { "FEEL_NO_PAIN", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-133.1f, -321.4f), Scale = 0.11f },
        }},
        { "FIEND_FIRE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-150.9f, -255.0f), Scale = 0.03f },
        }},
        { "FIGHT_ME", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-143.8f, -322.5f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-74.3f, -310.3f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-88.8f, -308.2f), Scale = 0.07f },
        }},
        { "FINISHER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-67.8f, -275.8f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-102.7f, -275.2f), Scale = 0.09f },
        }},
        { "FLAK_CANNON", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-233.9f, -298.3f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-233.0f, -311.0f), Scale = 0.03f },
        }},
        { "FLANKING", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-97.3f, -248.5f), Scale = 0.23f },
            new CardEyeConfig { Offset = new Vector2(-51.6f, -269.2f), Scale = 0.15f },
            new CardEyeConfig { Offset = new Vector2(-212.4f, -269.9f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-189.0f, -275.3f), Scale = 0.07f },
        }},
        { "FLICK_FLACK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-161.2f, -239.1f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-176.6f, -240.2f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-154.3f, -303.7f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-164.2f, -302.3f), Scale = 0.02f },
        }},
        { "FOLLOW_THROUGH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-86.9f, -251.4f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-119.9f, -219.0f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-171.3f, -317.9f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-150.7f, -325.7f), Scale = 0.08f },
        }},
        { "FOREGONE_CONCLUSION", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-213.4f, -333.3f), Scale = 0.12f },
            new CardEyeConfig { Offset = new Vector2(-148.6f, -328.1f), Scale = 0.08f },
        }},
        { "FORGOTTEN_RITUAL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-170.2f, -295.3f), Scale = 0.14f },
            new CardEyeConfig { Offset = new Vector2(-133.2f, -315.1f), Scale = 0.09f },
        }},
        { "FRIENDSHIP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-120.4f, -302.9f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-129.1f, -299.6f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-105.9f, -279.5f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-113.2f, -276.1f), Scale = 0.02f },
        }},
        { "GAMMA_BLAST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-171.0f, -318.3f), Scale = 0.25f },
        }},
        { "GLIMPSE_BEYOND", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-156.0f, -288.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-170.7f, -286.0f), Scale = 0.05f },
        }},
        { "GLITTERSTREAM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-90.5f, -330.1f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-197.5f, -249.8f), Scale = 0.06f },
        }},
        { "GO_FOR_THE_EYES", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-171.9f, -278.8f), Scale = 0.26f },
        }},
        { "GRAPPLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-202.6f, -357.7f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-175.7f, -362.3f), Scale = 0.07f },
        }},
        { "GRAVEBLAST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-242.5f, -293.5f), Scale = 0.01f },
            new CardEyeConfig { Offset = new Vector2(-239.9f, -294.0f), Scale = 0.01f },
        }},
        { "GUARDS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-199.4f, -293.9f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-184.7f, -295.3f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-133.6f, -297.3f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-113.1f, -298.8f), Scale = 0.08f },
        }},
        { "GUIDING_STAR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-74.3f, -305.1f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-94.5f, -293.2f), Scale = 0.08f },
        }},
        { "GUNK_UP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-125.0f, -313.3f), Scale = 0.12f },
        }},
        { "HANG", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-112.7f, -284.0f), Scale = 0.10f },
        }},
        { "HAUNT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-84.9f, -272.3f), Scale = 0.12f },
            new CardEyeConfig { Offset = new Vector2(-105.2f, -289.0f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-151.8f, -305.4f), Scale = 0.17f },
            new CardEyeConfig { Offset = new Vector2(-122.1f, -316.1f), Scale = 0.10f },
        }},
        { "HAVOC", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-167.3f, -322.1f), Scale = 0.08f },
        }},
        { "HAZE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-145.1f, -329.6f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-86.2f, -295.2f), Scale = 0.21f },
        }},
        { "HEADBUTT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-183.8f, -317.1f), Scale = 0.22f },
        }},
        { "HEAVENLY_DRILL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-190.9f, -285.4f), Scale = 0.11f },
        }},
        { "HEGEMONY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.9f, -348.5f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-48.6f, -242.9f), Scale = 0.02f },
        }},
        { "HEIRLOOM_HAMMER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-151.9f, -235.4f), Scale = 0.10f },
        }},
        { "HELIX_DRILL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-213.3f, -297.2f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-186.2f, -292.5f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-104.8f, -255.8f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-126.3f, -264.9f), Scale = 0.04f },
        }},
        { "HELLRAISER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-132.6f, -320.9f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-149.5f, -322.6f), Scale = 0.06f },
        }},
        { "HOLOGRAM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-233.8f, -231.2f), Scale = 0.10f, Opacity = 0.45f },
            new CardEyeConfig { Offset = new Vector2(-244.7f, -247.1f), Scale = 0.06f, Opacity = 0.45f },
            new CardEyeConfig { Offset = new Vector2(-192.9f, -257.3f), Scale = 0.14f, Opacity = 0.45f },
            new CardEyeConfig { Offset = new Vector2(-208.3f, -276.1f), Scale = 0.09f, Opacity = 0.45f },
            new CardEyeConfig { Offset = new Vector2(-148.7f, -291.6f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-168.7f, -318.8f), Scale = 0.10f },
        }},
        { "HUDDLE_UP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-201.2f, -308.9f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-183.8f, -335.9f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-172.2f, -338.0f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-126.2f, -333.6f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-115.2f, -334.0f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-81.1f, -304.5f), Scale = 0.08f },
        }},
        { "HYPERBEAM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-221.5f, -347.0f), Scale = 0.01f },
        }},
        { "I_AM_INVINCIBLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-188.6f, -286.1f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-64.1f, -329.7f), Scale = 0.10f },
        }},
        { "IGNITION", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-173.5f, -284.7f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-167.0f, -301.3f), Scale = 0.05f },
        }},
        { "IMPERVIOUS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-145.2f, -353.0f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-136.7f, -352.9f), Scale = 0.03f },
        }},
        { "INFERNO", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-151.8f, -325.4f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-147.2f, -325.5f), Scale = 0.02f },
        }},
        { "INTERCEPT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-53.9f, -341.7f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-142.3f, -291.2f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-136.2f, -291.3f), Scale = 0.02f },
        }},
        { "JUGGERNAUT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-199.9f, -330.2f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-189.4f, -330.6f), Scale = 0.04f },
        }},
        { "JUGGLING", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-198.4f, -276.1f), Scale = 0.04f },
        }},
        { "KINGLY_KICK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-182.1f, -354.9f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-225.4f, -273.0f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-219.5f, -273.0f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-238.8f, -244.1f), Scale = 0.06f },
        }},
        { "KINGLY_PUNCH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-140.7f, -322.1f), Scale = 0.10f },
        }},
        { "KNOCKOUT_BLOW", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-209.3f, -269.8f), Scale = 0.16f },
        }},
        { "KNOW_THY_PLACE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-124.6f, -288.8f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-117.6f, -294.1f), Scale = 0.04f },
        }},
        { "LARGESSE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-121.0f, -333.8f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-128.3f, -332.2f), Scale = 0.03f },
        }},
        { "LEAP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-171.3f, -331.0f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-179.6f, -338.5f), Scale = 0.03f },
        }},
        { "LEG_SWEEP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-95.9f, -313.2f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-107.6f, -315.2f), Scale = 0.03f },
        }},
        { "LEGION_OF_BONE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-84.9f, -285.1f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-72.5f, -291.5f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-193.7f, -318.2f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-189.2f, -318.9f), Scale = 0.02f },
        }},
        { "LETHALITY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-141.8f, -335.4f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-134.2f, -331.6f), Scale = 0.03f },
        }},
        { "LIGHTNING_ROD", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-200.0f, -275.9f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-211.0f, -289.6f), Scale = 0.04f },
        }},
        { "LUMINESCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-167.0f, -305.6f), Scale = 0.50f },
        }},
        { "MAKE_IT_SO", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-176.7f, -305.5f), Scale = 0.16f },
        }},
        { "MALAISE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-126.2f, -254.3f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-104.6f, -255.1f), Scale = 0.08f },
        }},
        { "MANGLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-153.8f, -276.3f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-128.5f, -289.0f), Scale = 0.07f },
        }},
        { "MANIFEST_AUTHORITY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-201.9f, -317.9f), Scale = 0.10f },
        }},
        { "MAUL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.1f, -250.5f), Scale = 0.60f },
            new CardEyeConfig { Offset = new Vector2(-147.1f, -352.5f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-125.1f, -352.5f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-82.8f, -338.4f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-227.8f, -328.2f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-242.7f, -339.4f), Scale = 0.08f },
        }},
        { "MELANCHOLY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-217.0f, -283.4f), Scale = 0.10f },
        }},
        { "MEMENTO_MORI", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-95.6f, -292.1f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-110.3f, -286.4f), Scale = 0.06f },
        }},
        { "MIMIC", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-224.2f, -313.1f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-107.0f, -279.5f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-90.3f, -279.7f), Scale = 0.05f },
        }},
        { "MINION_SACRIFICE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-95.2f, -300.3f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-176.6f, -327.7f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-204.9f, -306.4f), Scale = 0.11f },
        }},
        { "MINION_STRIKE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-133.6f, -305.1f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-145.8f, -307.5f), Scale = 0.05f },
        }},
        { "MISERY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-69.6f, -326.8f), Scale = 0.08f },
        }},
        { "MOMENTUM_STRIKE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-97.2f, -303.6f), Scale = 0.15f },
            new CardEyeConfig { Offset = new Vector2(-84.0f, -327.5f), Scale = 0.08f },
        }},
        { "MONARCHS_GAZE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-148.3f, -297.7f), Scale = 0.16f },
            new CardEyeConfig { Offset = new Vector2(-179.1f, -300.6f), Scale = 0.16f },
        }},
        { "MONOLOGUE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-207.4f, -269.0f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-129.3f, -321.7f), Scale = 0.24f },
        }},
        { "NEOWS_FURY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-283.6f, -316.7f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-275.2f, -293.4f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-254.1f, -248.9f), Scale = 0.34f },
            new CardEyeConfig { Offset = new Vector2(-67.4f, -248.8f), Scale = 0.34f },
            new CardEyeConfig { Offset = new Vector2(-43.0f, -293.2f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-34.6f, -315.5f), Scale = 0.08f },
        }},
        { "NEUROSURGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-209.4f, -280.6f), Scale = 0.15f },
        }},
        { "NIGHTMARE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-142.7f, -263.8f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-184.4f, -256.3f), Scale = 0.20f },
        }},
        { "NO_ESCAPE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-122.3f, -302.9f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-105.0f, -306.3f), Scale = 0.07f },
        }},
        { "NOXIOUS_FUMES", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-131.5f, -229.7f), Scale = 0.11f },
        }},
        { "OBLIVION", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-136.1f, -328.9f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-129.5f, -327.8f), Scale = 0.03f },
        }},
        { "OFFERING", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-171.0f, -297.5f), Scale = 0.15f },
            new CardEyeConfig { Offset = new Vector2(-136.5f, -304.0f), Scale = 0.15f },
        }},
        { "OUTMANEUVER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-161.3f, -322.9f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-188.6f, -319.3f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-123.3f, -301.9f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-140.5f, -308.7f), Scale = 0.05f },
        }},
        { "PACTS_END", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-68.6f, -348.8f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-75.6f, -348.2f), Scale = 0.03f },
        }},
        { "PAGESTORM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-167.4f, -308.1f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-184.4f, -293.1f), Scale = 0.07f },
        }},
        { "PALE_BLUE_DOT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-148.9f, -352.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-41.8f, -294.1f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-60.0f, -302.6f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-89.3f, -323.1f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-104.0f, -321.4f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-210.4f, -323.6f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-196.7f, -320.0f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-260.6f, -293.2f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-241.1f, -304.1f), Scale = 0.07f },
        }},
        { "PARRY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-221.2f, -296.3f), Scale = 0.10f },
        }},
        { "PARSE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-120.0f, -255.9f), Scale = 0.19f },
            new CardEyeConfig { Offset = new Vector2(-176.3f, -263.5f), Scale = 0.17f },
        }},
        { "PARTICLE_WALL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-195.7f, -308.4f), Scale = 0.23f },
        }},
        { "PATTER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-88.0f, -294.6f), Scale = 0.12f },
        }},
        { "PECK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.3f, -314.9f), Scale = 0.20f },
        }},
        { "PERFECTED_STRIKE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-76.7f, -249.8f), Scale = 0.07f },
        }},
        { "PHANTOM_BLADES", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-158.2f, -363.9f), Scale = 0.06f },
        }},
        { "PHOTON_CUT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-167.8f, -302.2f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-191.0f, -296.5f), Scale = 0.10f },
        }},
        { "PIERCING_WAIL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-137.5f, -320.6f), Scale = 0.10f },
        }},
        { "POUNCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-127.5f, -241.0f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-165.9f, -299.6f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-177.7f, -298.9f), Scale = 0.03f },
        }},
        { "PRECISE_CUT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-187.5f, -345.5f), Scale = 0.16f },
            new CardEyeConfig { Offset = new Vector2(-149.3f, -341.4f), Scale = 0.16f },
        }},
        { "PRIMAL_FORCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-144.5f, -306.7f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-131.8f, -304.8f), Scale = 0.06f },
        }},
        { "PROPHESIZE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-168.1f, -309.5f), Scale = 0.10f },
        }},
        { "PROTECTOR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-148.3f, -278.4f), Scale = 0.03f },
        }},
        { "QUADCAST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-244.7f, -308.8f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-245.3f, -325.4f), Scale = 0.05f },
        }},
        { "QUASAR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-133.7f, -328.9f), Scale = 0.20f },
        }},
        { "RALLY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-118.8f, -270.6f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-136.5f, -346.2f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-126.1f, -342.4f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-197.1f, -269.3f), Scale = 0.10f },
        }},
        { "RAMPAGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-257.2f, -269.0f), Scale = 0.05f },
        }},
        { "REAP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-204.7f, -298.1f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-193.7f, -292.5f), Scale = 0.04f },
        }},
        { "REAPER_FORM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.9f, -310.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-146.7f, -312.9f), Scale = 0.05f },
        }},
        { "REAVE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-64.4f, -305.8f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-80.4f, -310.6f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-153.3f, -340.6f), Scale = 0.08f, Opacity = 0.60f },
            new CardEyeConfig { Offset = new Vector2(-134.7f, -338.6f), Scale = 0.08f, Opacity = 0.60f },
        }},
        { "REFINE_BLADE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-113.5f, -272.0f), Scale = 0.20f },
        }},
        { "REFLECT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-184.4f, -303.1f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-173.3f, -298.7f), Scale = 0.05f },
        }},
        { "REFLEX", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-212.0f, -318.1f), Scale = 0.10f },
        }},
        { "REFRACT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-57.4f, -260.3f), Scale = 0.05f },
        }},
        { "REGRET", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-176.2f, -317.3f), Scale = 0.06f },
        }},
        { "RIGHT_HAND_HAND", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-88.3f, -332.6f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-208.2f, -313.5f), Scale = 0.06f },
        }},
        { "ROCKET_PUNCH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-224.9f, -241.2f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-226.6f, -233.4f), Scale = 0.05f },
        }},
        { "ROLLING_BOULDER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-58.8f, -277.2f), Scale = 0.10f },
        }},
        { "ROYAL_GAMBLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-149.4f, -304.2f), Scale = 0.30f },
        }},
        { "ROYALTIES", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-168.7f, -298.3f), Scale = 0.16f },
            new CardEyeConfig { Offset = new Vector2(-211.1f, -304.1f), Scale = 0.16f },
        }},
        { "RUPTURE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-138.8f, -323.6f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-158.8f, -322.9f), Scale = 0.08f },
        }},
        { "SACRIFICE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-199.1f, -339.7f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-191.9f, -341.5f), Scale = 0.03f },
        }},
        { "SALVO", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-53.7f, -309.5f), Scale = 0.08f },
        }},
        { "SCOURGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-102.8f, -298.7f), Scale = 0.12f },
        }},
        { "SCRAPE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-242.0f, -308.7f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-198.5f, -309.7f), Scale = 0.24f },
        }},
        { "SCULPTING_STRIKE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-53.8f, -266.7f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-81.8f, -266.1f), Scale = 0.09f },
        }},
        { "SECOND_WIND", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-149.6f, -350.2f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-173.6f, -350.4f), Scale = 0.11f },
        }},
        { "SEEKING_EDGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-151.4f, -245.2f), Scale = 0.14f },
        }},
        { "SENTRY_MODE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-215.7f, -256.8f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-176.5f, -328.9f), Scale = 0.15f },
            new CardEyeConfig { Offset = new Vector2(-48.6f, -323.1f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-62.8f, -319.7f), Scale = 0.04f },
        }},
        { "SERPENT_FORM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-135.4f, -288.6f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-146.1f, -294.2f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-79.5f, -312.4f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-104.9f, -292.5f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-224.7f, -335.2f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-191.7f, -306.4f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-126.5f, -359.3f), Scale = 0.05f },
        }},
        { "SETUP_STRIKE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-118.0f, -339.3f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-73.3f, -324.9f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-93.9f, -332.9f), Scale = 0.07f },
        }},
        { "SEVEN_STARS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-134.7f, -303.6f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-159.9f, -311.7f), Scale = 0.10f },
        }},
        { "SEVERANCE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.3f, -355.4f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-148.8f, -363.1f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-159.6f, -277.7f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-142.0f, -280.1f), Scale = 0.08f },
        }},
        { "SHADOWMELD", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-163.9f, -298.5f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-140.7f, -299.8f), Scale = 0.12f },
        }},
        { "SHROUD", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-140.4f, -301.4f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-158.4f, -301.4f), Scale = 0.06f },
        }},
        { "SHRUG_IT_OFF", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-155.2f, -322.6f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-133.4f, -319.9f), Scale = 0.09f },
        }},
        { "SIGNAL_BOOST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-142.5f, -263.6f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-155.2f, -231.7f), Scale = 0.20f },
        }},
        { "SKEWER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-149.6f, -334.3f), Scale = 0.10f },
        }},
        { "SLICE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-160.0f, -333.9f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-124.2f, -339.1f), Scale = 0.10f },
        }},
        { "SNAKEBITE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-241.1f, -280.3f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-254.9f, -280.8f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-121.5f, -355.1f), Scale = 0.10f },
        }},
        { "SNEAKY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-169.6f, -303.9f), Scale = 0.05f },
        }},
        { "SOLAR_STRIKE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-215.7f, -305.2f), Scale = 0.13f },
        }},
        { "SOUL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-159.3f, -333.5f), Scale = 0.14f },
            new CardEyeConfig { Offset = new Vector2(-128.5f, -327.4f), Scale = 0.12f },
        }},
        { "SOUL_STORM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-178.4f, -323.9f), Scale = 0.08f },
        }},
        { "SOW", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-117.8f, -346.6f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-111.0f, -342.5f), Scale = 0.03f },
        }},
        { "SPIRIT_OF_ASH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-134.8f, -321.8f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-145.5f, -324.0f), Scale = 0.06f },
        }},
        { "SPOILS_OF_BATTLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-223.9f, -289.0f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-207.3f, -300.0f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-89.8f, -296.9f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-75.0f, -289.8f), Scale = 0.08f },
        }},
        { "SPUR", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-187.9f, -297.7f), Scale = 0.10f },
        }},
        { "SQUEEZE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-117.5f, -292.6f), Scale = 0.27f },
        }},
        { "STAMPEDE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-164.3f, -359.3f), Scale = 0.04f },
        }},
        { "STOMP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-96.4f, -269.3f), Scale = 0.13f },
        }},
        { "STRANGLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-174.2f, -328.2f), Scale = 0.14f },
            new CardEyeConfig { Offset = new Vector2(-123.3f, -325.1f), Scale = 0.16f },
        }},
        { "SUCKER_PUNCH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-57.8f, -291.0f), Scale = 0.20f },
            new CardEyeConfig { Offset = new Vector2(-107.5f, -316.6f), Scale = 0.30f },
        }},
        { "SUMMON_FORTH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-82.2f, -300.7f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-109.6f, -299.0f), Scale = 0.14f },
            new CardEyeConfig { Offset = new Vector2(-214.5f, -257.4f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-204.3f, -258.7f), Scale = 0.04f },
        }},
        { "SUPERCRITICAL", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-117.3f, -304.3f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-129.1f, -298.0f), Scale = 0.08f },
        }},
        { "SUPPRESS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-103.0f, -315.0f), Scale = 0.07f },
        }},
        { "SWEEPING_BEAM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-103.6f, -234.6f), Scale = 0.16f },
            new CardEyeConfig { Offset = new Vector2(-196.2f, -333.9f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-198.7f, -321.6f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-83.2f, -344.6f), Scale = 0.07f },
        }},
        { "SWORD_BOOMERANG", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-210.2f, -329.6f), Scale = 0.08f },
        }},
        { "SWORD_SAGE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-169.4f, -293.7f), Scale = 0.12f },
        }},
        { "TAG_TEAM", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-188.5f, -259.6f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-199.1f, -252.6f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-109.3f, -262.5f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-119.8f, -254.7f), Scale = 0.08f },
        }},
        { "TANK", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-227.7f, -304.2f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-221.0f, -304.5f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-202.2f, -303.6f), Scale = 0.07f },
            new CardEyeConfig { Offset = new Vector2(-213.3f, -293.9f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-219.9f, -281.2f), Scale = 0.06f },
            new CardEyeConfig { Offset = new Vector2(-167.4f, -271.9f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-184.9f, -264.7f), Scale = 0.07f },
        }},
        { "TAUNT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-108.3f, -309.1f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-86.8f, -305.2f), Scale = 0.06f },
        }},
        { "TEMPEST", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-151.3f, -350.5f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-151.0f, -331.3f), Scale = 0.14f },
        }},
        { "TERRAFORMING", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-160.2f, -253.2f), Scale = 0.20f },
        }},
        { "THE_HUNT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-132.8f, -358.1f), Scale = 0.14f },
        }},
        { "THE_SCYTHE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-120.4f, -300.3f), Scale = 0.08f },
            new CardEyeConfig { Offset = new Vector2(-163.1f, -320.2f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-155.1f, -323.6f), Scale = 0.03f },
        }},
        { "THE_SMITH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-47.1f, -277.2f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-61.1f, -282.0f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-252.9f, -279.6f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-264.4f, -281.5f), Scale = 0.04f },
        }},
        { "THRASH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-77.8f, -293.8f), Scale = 0.10f },
            new CardEyeConfig { Offset = new Vector2(-112.6f, -299.7f), Scale = 0.08f },
        }},
        { "THUNDER", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-122.5f, -294.1f), Scale = 0.11f },
            new CardEyeConfig { Offset = new Vector2(-112.1f, -311.9f), Scale = 0.07f },
        }},
        { "TIMES_UP", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-134.9f, -333.7f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-169.0f, -334.6f), Scale = 0.09f },
        }},
        { "TRANSFIGURE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-199.8f, -274.6f), Scale = 0.12f },
            new CardEyeConfig { Offset = new Vector2(-170.2f, -268.6f), Scale = 0.10f },
        }},
        { "TREMBLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-159.7f, -294.4f), Scale = 0.20f },
        }},
        { "TRUE_GRIT", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-173.0f, -322.7f), Scale = 0.02f },
            new CardEyeConfig { Offset = new Vector2(-168.6f, -325.1f), Scale = 0.02f },
        }},
        { "TURBO", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-66.5f, -324.5f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-75.0f, -312.8f), Scale = 0.08f },
        }},
        { "TYRANNY", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-166.8f, -330.2f), Scale = 0.10f },
        }},
        { "UNDEATH", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-192.3f, -301.5f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-180.7f, -302.1f), Scale = 0.04f },
        }},
        { "UNMOVABLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-162.8f, -347.6f), Scale = 0.09f },
            new CardEyeConfig { Offset = new Vector2(-53.7f, -244.9f), Scale = 0.10f },
        }},
        { "UNTOUCHABLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-157.2f, -322.7f), Scale = 0.04f },
            new CardEyeConfig { Offset = new Vector2(-147.3f, -323.9f), Scale = 0.03f },
        }},
        { "VICIOUS", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-165.3f, -347.7f), Scale = 0.03f },
            new CardEyeConfig { Offset = new Vector2(-155.8f, -345.3f), Scale = 0.03f },
        }},
        { "WHISTLE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-111.0f, -311.1f), Scale = 0.05f },
            new CardEyeConfig { Offset = new Vector2(-95.1f, -317.9f), Scale = 0.04f },
        }},
        { "WHITE_NOISE", new CardEyeConfig[] {
            new CardEyeConfig { Offset = new Vector2(-71.7f, -317.0f), Scale = 0.13f },
            new CardEyeConfig { Offset = new Vector2(-72.7f, -285.6f), Scale = 0.18f },
        }},
    };
}
