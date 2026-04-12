using Godot;

namespace GooglyEyes;

public static class ScreenGooglyEyesRegistry
{
    public static readonly Dictionary<string, EyeConfig[]> Configs = new()
    {
        { "res://scenes/screens/char_select/char_select_bg_ironclad.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(200.5f, -70.2f), Scale = 1.30f, AnchorBone = "bod4" },
            new EyeConfig { Offset = new Vector2(209.4f, -345.6f), Scale = 1.30f, AnchorBone = "bod4" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_defect.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(476.5f, 23.7f), Scale = 0.65f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(222.6f, 25.6f), Scale = 1.57f, AnchorBone = "head" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_necrobinder.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(123.7f, -64.6f), Scale = 0.85f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(115.6f, -251.8f), Scale = 0.78f, AnchorBone = "head" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_regent.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(11.7f, -7.8f), Scale = 0.87f, AnchorBone = "eye" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_silent.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-423.4f, -428.1f), Scale = 0.87f, AnchorBone = "shoulder_r" },
        }},
        { "res://scenes/events/background_scenes/darv.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(473.4f, 237.4f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(866.6f, 331.7f), Scale = 0.18f },
            new EyeConfig { Offset = new Vector2(809.6f, 330.4f), Scale = 0.18f },
        }},
        { "res://scenes/events/background_scenes/neow.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-7.2f, -7.1f), Scale = 0.55f, AnchorBone = "eye a 1" },
            new EyeConfig { Offset = new Vector2(3.2f, -3.1f), Scale = 0.55f, AnchorBone = "eye b 1" },
            new EyeConfig { Offset = new Vector2(2.1f, -3.2f), Scale = 0.55f, AnchorBone = "eye c 1" },
        }},
        { "res://scenes/events/background_scenes/orobas.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(869.0f, 382.5f), Scale = 1.83f },
        }},
        { "res://scenes/events/background_scenes/pael.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1065.1f, 426.7f), Scale = 0.50f },
        }},
        { "res://scenes/events/background_scenes/tanx.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(874.4f, 225.8f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1238.8f, 300.6f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1276.1f, 400.7f), Scale = 0.13f },
        }},
        { "res://scenes/events/background_scenes/tezcatara.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(4.7f, 9.7f), Scale = 0.99f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(1.2f, -5.0f), Scale = 0.99f, AnchorBone = "eye_r" },
        }},
        { "res://images/events/amalgamator.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1116.5f, 762.6f), Scale = 0.27f },
            new EyeConfig { Offset = new Vector2(1175.8f, 771.0f), Scale = 0.27f },
        }},
        { "res://images/events/battleworn_dummy.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1118.3f, 877.6f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(1091.6f, 892.6f), Scale = 0.14f },
        }},
        { "res://images/events/brain_leech.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1127.1f, 996.4f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(1032.2f, 1008.8f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(1002.6f, 1065.1f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(1163.3f, 1053.1f), Scale = 0.30f },
        }},
        { "res://images/events/bugslayer.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1176.3f, 813.0f), Scale = 0.06f },
            new EyeConfig { Offset = new Vector2(1159.2f, 813.6f), Scale = 0.06f },
            new EyeConfig { Offset = new Vector2(1178.9f, 825.7f), Scale = 0.07f },
            new EyeConfig { Offset = new Vector2(1154.8f, 825.3f), Scale = 0.07f },
            new EyeConfig { Offset = new Vector2(1134.3f, 843.1f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1183.9f, 850.7f), Scale = 0.20f },
        }},
        { "res://images/events/colorful_philosophers.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1036.3f, 677.6f), Scale = 0.07f },
            new EyeConfig { Offset = new Vector2(1060.8f, 679.6f), Scale = 0.07f },
            new EyeConfig { Offset = new Vector2(1240.1f, 631.1f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1217.8f, 641.2f), Scale = 0.10f },
        }},
        { "res://images/events/crystal_sphere.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1114.9f, 766.8f), Scale = 0.22f },
            new EyeConfig { Offset = new Vector2(1167.1f, 766.7f), Scale = 0.22f },
        }},
        { "res://images/events/doll_room.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1073.9f, 485.8f), Scale = 0.46f },
            new EyeConfig { Offset = new Vector2(1223.5f, 471.8f), Scale = 0.51f },
            new EyeConfig { Offset = new Vector2(942.4f, 797.3f), Scale = 0.17f },
            new EyeConfig { Offset = new Vector2(876.4f, 796.7f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(1098.1f, 807.3f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(1017.1f, 845.9f), Scale = 0.22f },
            new EyeConfig { Offset = new Vector2(1200.7f, 761.8f), Scale = 0.13f },
            new EyeConfig { Offset = new Vector2(1161.3f, 761.8f), Scale = 0.13f },
            new EyeConfig { Offset = new Vector2(1328.1f, 771.5f), Scale = 0.18f },
            new EyeConfig { Offset = new Vector2(1283.5f, 774.0f), Scale = 0.18f },
            new EyeConfig { Offset = new Vector2(1444.3f, 793.1f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(1486.0f, 798.5f), Scale = 0.09f },
            new EyeConfig { Offset = new Vector2(921.3f, 999.1f), Scale = 0.16f },
            new EyeConfig { Offset = new Vector2(882.9f, 995.4f), Scale = 0.13f },
        }},
        { "res://images/events/infested_automaton.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1139.1f, 813.9f), Scale = 0.37f },
            new EyeConfig { Offset = new Vector2(1027.5f, 819.6f), Scale = 0.37f },
        }},
        { "res://images/events/morphic_grove.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(812.9f, 592.9f), Scale = 0.05f },
            new EyeConfig { Offset = new Vector2(823.2f, 597.7f), Scale = 0.05f },
            new EyeConfig { Offset = new Vector2(1044.1f, 871.2f), Scale = 0.09f },
            new EyeConfig { Offset = new Vector2(1056.1f, 887.0f), Scale = 0.09f },
            new EyeConfig { Offset = new Vector2(1258.8f, 886.5f), Scale = 0.12f },
            new EyeConfig { Offset = new Vector2(1288.6f, 888.8f), Scale = 0.12f },
            new EyeConfig { Offset = new Vector2(1260.7f, 704.2f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1283.9f, 705.3f), Scale = 0.10f },
        }},
        { "res://images/events/potion_courier.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(965.8f, 749.0f), Scale = 0.30f },
        }},
        { "res://images/events/ranwid_the_elder.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1160.9f, 858.4f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(1112.3f, 855.7f), Scale = 0.14f },
        }},
        { "res://images/events/relic_trader.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1010.6f, 689.6f), Scale = 0.15f },
        }},
        { "res://images/events/round_tea_party.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1109.2f, 878.7f), Scale = 0.17f },
            new EyeConfig { Offset = new Vector2(1149.5f, 879.0f), Scale = 0.17f },
            new EyeConfig { Offset = new Vector2(1415.3f, 1015.7f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1388.0f, 1014.9f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1639.9f, 1045.7f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1955.1f, 1060.2f), Scale = 0.11f },
            new EyeConfig { Offset = new Vector2(1931.1f, 1061.4f), Scale = 0.11f },
        }},
        { "res://images/events/self_help_book.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1216.1f, 960.4f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1193.8f, 963.8f), Scale = 0.10f },
        }},
        { "res://images/events/stone_of_all_time.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1021.7f, 634.9f), Scale = 0.90f },
            new EyeConfig { Offset = new Vector2(1262.1f, 614.0f), Scale = 0.90f },
        }},
        { "res://images/events/tea_master.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1108.6f, 753.6f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(1174.2f, 755.7f), Scale = 0.30f },
        }},
        { "res://images/events/tinker_time.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1220.3f, 950.1f), Scale = 0.23f },
            new EyeConfig { Offset = new Vector2(1272.7f, 937.9f), Scale = 0.23f },
        }},
        { "res://images/events/war_historian_repy.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1036.2f, 801.9f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(1085.4f, 799.3f), Scale = 0.14f },
        }},
        { "res://images/events/welcome_to_wongos.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1104.0f, 875.4f), Scale = 0.24f },
        }},
        { "res://images/events/wood_carvings.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1048.7f, 897.5f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(878.8f, 822.7f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(848.3f, 818.3f), Scale = 0.10f },
        }},
        { "res://images/events/zen_weaver.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1183.2f, 879.6f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1143.8f, 893.1f), Scale = 0.20f },
        }},
        
        // Acts From The Past
        
        { "res://images/events/augmenter.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1095.0f, 581.2f), Scale = 0.60f },
        }},
        { "res://images/events/big_fish.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1326.7f, 878.6f), Scale = 0.40f },
            new EyeConfig { Offset = new Vector2(1189.3f, 886.2f), Scale = 0.40f },
        }},
        { "res://images/events/cleric.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1084.4f, 693.0f), Scale = 0.40f },
            new EyeConfig { Offset = new Vector2(967.0f, 693.0f), Scale = 0.40f },
        }},
        { "res://images/events/council_of_ghosts.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(787.6f, 723.7f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(815.0f, 737.0f), Scale = 0.14f },
            new EyeConfig { Offset = new Vector2(988.3f, 631.4f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1033.1f, 628.8f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1094.4f, 838.3f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1052.5f, 849.5f), Scale = 0.20f },
        }},
        { "res://images/events/forgotten_altar.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1057.2f, 611.1f), Scale = 0.13f },
            new EyeConfig { Offset = new Vector2(1024.1f, 608.8f), Scale = 0.13f },
        }},
        { "res://images/events/golden_idol.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1059.1f, 603.7f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1012.5f, 600.6f), Scale = 0.20f },
        }},
        { "res://images/events/knowing_skull.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1052.0f, 597.3f), Scale = 0.50f },
            new EyeConfig { Offset = new Vector2(1162.1f, 599.0f), Scale = 0.50f },
        }},
        { "res://images/events/living_wall.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1149.1f, 666.1f), Scale = 0.40f },
            new EyeConfig { Offset = new Vector2(994.2f, 673.6f), Scale = 0.40f },
            new EyeConfig { Offset = new Vector2(1356.2f, 958.6f), Scale = 0.40f },
            new EyeConfig { Offset = new Vector2(1270.4f, 972.6f), Scale = 0.40f },
            new EyeConfig { Offset = new Vector2(717.5f, 800.0f), Scale = 0.37f },
            new EyeConfig { Offset = new Vector2(792.7f, 822.4f), Scale = 0.37f },
        }},
        { "res://images/events/mind_bloom.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1105.0f, 1043.5f), Scale = 0.50f },
            new EyeConfig { Offset = new Vector2(966.8f, 1045.3f), Scale = 0.50f },
        }},
        { "res://images/events/moai_head.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1139.2f, 515.5f), Scale = 0.60f },
            new EyeConfig { Offset = new Vector2(962.3f, 501.8f), Scale = 0.60f },
        }},
        { "res://images/events/nloth.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1095.7f, 676.6f), Scale = 0.60f },
            new EyeConfig { Offset = new Vector2(973.3f, 670.0f), Scale = 0.60f },
        }},
        { "res://images/events/pleading_vagrant.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1159.0f, 777.5f), Scale = 0.60f },
            new EyeConfig { Offset = new Vector2(1031.0f, 748.5f), Scale = 0.60f },
        }},
        { "res://images/events/sssserpent.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(986.4f, 711.1f), Scale = 0.50f },
            new EyeConfig { Offset = new Vector2(819.8f, 715.8f), Scale = 0.50f },
        }},
        { "res://images/events/vampires.png", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1137.4f, 544.9f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(1058.4f, 546.5f), Scale = 0.30f },
        }},
    };
}