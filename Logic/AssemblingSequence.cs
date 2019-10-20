using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MozaicLand
{
    public interface IAssemblingAction
    {
        // Returns time after action execution
        TimeSpan Execute(TimeSpan currentTime);
    }

    // TODO: exchange of cartridges ?

    public class GetBlockAction : IAssemblingAction
    {
        public Robot Robot { get; set; }
        public ColorCartridgeSlot Target { get; set; }

        public GetBlockAction(Robot robot, ColorCartridgeSlot target)
        {
            Robot = robot;
            Target = target;
        }

        public TimeSpan Execute(TimeSpan currentTime)
        {
            if (Target.Cartridge.CurrentCount <= 0)
            {
                throw new InvalidOperationException("Attempt to get block from empty cartridge");
            }

            currentTime += Robot.MoveTo(Target.CatchHole);
            currentTime += Robot.CatchBlock(Target.Cartridge.ColorIndex);
            Target.Cartridge.CurrentCount -= 1;
            return currentTime;
        }
    }

    public class PutBlockAction : IAssemblingAction
    {
        public Robot Robot { get; set; }
        public PalletSlot Pallet { get; set; }
        public Point Block { get; set; }

        public PutBlockAction(Robot robot, PalletSlot pallet, Point block)
        {
            Robot = robot;
            Pallet = pallet;
            Block = block;
        }

        public TimeSpan Execute(TimeSpan currentTime)
        {
            if (Pallet.Pallet.BlocksColors[Block.Y, Block.X] >= 0)
            {
                throw new InvalidOperationException("Attempt to put block into already filled cell");
            }

            PointF blockPos = Pallet.TopLeft.Add(Pallet.Pallet.BlockCenter(Block.Y, Block.X));
            currentTime += Robot.MoveTo(blockPos);
            Pallet.Pallet.BlocksColors[Block.Y, Block.X] = Robot.CurrentBlock;
            currentTime += Robot.PutBlock();
            return currentTime;
        }
    }

    public class AssemblingSequence
    {
        public WorkingSpace WorkingSpace { get; set; }
        public List<IAssemblingAction> Actions { get; set; }

        public int NextAction { get; private set; } = 0;
        public TimeSpan ExecutionTime { get; set; } = TimeSpan.Zero;

        public void ExecuteNext()
        {
            if(NextAction < Actions.Count)
            {
                IAssemblingAction action = Actions[NextAction];
                ExecutionTime = action.Execute(ExecutionTime);
                NextAction++;
            }
        }

        public void ExecuteAll()
        {
            while (NextAction < Actions.Count)
            {
                ExecuteNext();
            }
        }
    }
}
