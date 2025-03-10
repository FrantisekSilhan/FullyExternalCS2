using CS2Cheat.Data.Game;
using CS2Cheat.Utils;
using SharpDX;

namespace CS2Cheat.Data.Entity;

public class Entity(int index) : EntityBase
{
    private int Index { get; } = index;

    private bool Dormant { get; set; } = true;

    protected internal bool IsSpotted { get; private set; }

    protected internal string Name { get; private set; } = null!;

    protected internal int IsinScope { get; private set; }

    protected internal int FlashAlpha { get; private set; }


	public enum Bone {
		head,
		neck_0,
		spine_1,
		spine_2,
		pelvis,
		arm_upper_L,
		arm_lower_L,
		hand_L,
		arm_upper_R,
		arm_lower_R,
		hand_R,
		leg_upper_L,
		leg_lower_L,
		ankle_L,
		leg_upper_R,
		leg_lower_R,
		ankle_R
	}
	public Dictionary<string, Vector3> BonePos { get; } = new()
    {
        { Bone.head.ToString(), Vector3.Zero },
        { Bone.neck_0.ToString(), Vector3.Zero },
        { Bone.spine_1.ToString(), Vector3.Zero },
        { Bone.spine_2.ToString(), Vector3.Zero },
        { Bone.pelvis.ToString(), Vector3.Zero },
        { Bone.arm_upper_L.ToString(), Vector3.Zero },
        { Bone.arm_lower_L.ToString(), Vector3.Zero },
        { Bone.hand_L.ToString(), Vector3.Zero },
        { Bone.arm_upper_R.ToString(), Vector3.Zero },
        { Bone.arm_lower_R.ToString(), Vector3.Zero },
        { Bone.hand_R.ToString(), Vector3.Zero },
        { Bone.leg_upper_L.ToString(), Vector3.Zero },
        { Bone.leg_lower_L.ToString(), Vector3.Zero },
        { Bone.ankle_L.ToString(), Vector3.Zero },
        { Bone.leg_upper_R.ToString(), Vector3.Zero },
        { Bone.leg_lower_R.ToString(), Vector3.Zero },
        { Bone.ankle_R.ToString(), Vector3.Zero }
	};

    public override bool IsAlive()
    {
        return base.IsAlive() && !Dormant;
    }

    protected override IntPtr ReadControllerBase(GameProcess gameProcess)
    {
        var listEntryFirst = gameProcess.Process.Read<IntPtr>(EntityList + ((8 * (Index & 0x7FFF)) >> 9) + 16);
        return listEntryFirst == IntPtr.Zero
            ? IntPtr.Zero
            : gameProcess.Process.Read<IntPtr>(listEntryFirst + 120 * (Index & 0x1FF));
    }

    protected override IntPtr ReadAddressBase(GameProcess gameProcess)
    {
        var playerPawn = gameProcess.Process.Read<int>(ControllerBase + Offsets.m_hPawn);
        var listEntrySecond = gameProcess.Process.Read<IntPtr>(EntityList + 0x8 * ((playerPawn & 0x7FFF) >> 9) + 16);
        return listEntrySecond == IntPtr.Zero
            ? IntPtr.Zero
            : gameProcess.Process.Read<IntPtr>(listEntrySecond + 120 * (playerPawn & 0x1FF));
    }

    public override bool Update(GameProcess gameProcess)
    {
        if (!base.Update(gameProcess)) return false;

        Dormant = gameProcess.Process.Read<bool>(AddressBase + Offsets.m_bDormant);
        IsSpotted = gameProcess.Process.Read<bool>(AddressBase + Offsets.m_entitySpottedState + 0x8);
        IsinScope = gameProcess.Process.Read<int>(AddressBase + Offsets.m_bIsScoped);
        FlashAlpha = gameProcess.Process.Read<int>(AddressBase + Offsets.m_flFlashDuration);
        Name = gameProcess.Process.ReadString(ControllerBase + Offsets.m_iszPlayerName);
        if (!IsAlive()) return true;

        UpdateBonePos(gameProcess);

        return true;
    }

    private void UpdateBonePos(GameProcess gameProcess)
    {
        var gameSceneNode = gameProcess.Process.Read<IntPtr>(AddressBase + Offsets.m_pGameSceneNode);
        var boneArray = gameProcess.Process.Read<IntPtr>(gameSceneNode + Offsets.m_modelState + 128);
        foreach (var (name, index) in Offsets.Bones)
        {
            var boneAddress = boneArray + index * 32;
            var bonePos = gameProcess.Process.Read<Vector3>(boneAddress);
            BonePos[name] = bonePos;
        }
    }
}