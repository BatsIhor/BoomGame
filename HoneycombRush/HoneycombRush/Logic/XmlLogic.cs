using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using HoneycombRush.Objects;
using HoneycombRush.ScreenManagerLogic;
using HoneycombRush.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush.Logic
{
    internal class XmlLogic
    {
        /// <summary>
        /// Loads animation settings from an xml file.
        /// </summary>
        public static Dictionary<string, Animation> LoadAnimationFromXml(ScreenManager screenManager)
        {
            XDocument doc = XDocument.Load("Content/Textures/AnimationsDefinition.xml");

            Dictionary<string, Animation> animations = new Dictionary<string, Animation>();

            XName name = XName.Get("Definition");
            if (doc.Document != null)
            {
                var definitions = doc.Document.Descendants(name);

                // Loop over all definitions in the XML
                foreach (var animationDefinition in definitions)
                {
                    // Get the name of the animation
                    string animationAlias = animationDefinition.Attribute("Alias").Value;
                    Texture2D texture =
                        screenManager.Game.Content.Load<Texture2D>(animationDefinition.Attribute("SheetName").Value);

                    // Get the frame size (width & height)
                    Point frameSize = new Point();
                    frameSize.X = int.Parse(animationDefinition.Attribute("FrameWidth").Value);
                    frameSize.Y = int.Parse(animationDefinition.Attribute("FrameHeight").Value);

                    // Get the frames sheet dimensions
                    Point sheetSize = new Point();
                    sheetSize.X = int.Parse(animationDefinition.Attribute("SheetColumns").Value);
                    sheetSize.Y = int.Parse(animationDefinition.Attribute("SheetRows").Value);

                    Animation animation = new Animation(texture, frameSize, sheetSize);

                    // Checks for sub-animation definition
                    if (animationDefinition.Element("SubDefinition") != null)
                    {
                        int startFrame =
                            int.Parse(animationDefinition.Element("SubDefinition").Attribute("StartFrame").Value);

                        int endFrame =
                            int.Parse(animationDefinition.Element("SubDefinition").Attribute("EndFrame").Value);

                        animation.SetSubAnimation(startFrame, endFrame);
                    }

                    if (animationDefinition.Attribute("Speed") != null)
                    {
                        animation.SetFrameInterval(
                            TimeSpan.FromMilliseconds(double.Parse(animationDefinition.Attribute("Speed").Value)));
                    }

                    if (null != animationDefinition.Attribute("OffsetX") &&
                        null != animationDefinition.Attribute("OffsetY"))
                    {
                        animation.Offset = new Vector2(int.Parse(animationDefinition.Attribute("OffsetX").Value),
                                                       int.Parse(animationDefinition.Attribute("OffsetY").Value));
                    }

                    animations.Add(animationAlias, animation);
                }
            }
            return animations;
        }

        /// <summary>
        /// Creates all the blocks and bees.
        /// </summary>
        public static Block[,] CreateLevel(ScreenManager screenManager, Texture2D blockTexture,
                                           Dictionary<string, Animation> animations, GameplayScreen gamePlay)
        {
            Block[,] blocks = new Block[15,11];

            // TODO read level from file.
            List<Block> blocksList = new List<Block>
                                         {
                                             //First line
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(60, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(100, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(140, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(180, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(220, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(260, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(340, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(380, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(420, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(460, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(500, 104)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(540, 104)),
                    
                                             //second line
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(60, 140)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(540, 140)),
            
                                             //third line
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(60, 176)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(540, 176)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(60, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(100, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(140, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(180, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(220, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(260, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(300, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(340, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(380, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(420, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(460, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(500, 212)),
                                             new Block(screenManager.Game, gamePlay, blockTexture, new Vector2(540, 212)),
                                         };

            foreach (Block block in blocksList)
            {
                block.AnimationDefinitions = animations;
                screenManager.Game.Components.Add(block);

                Vector2 vector = new Vector2((block.BodyRectangle.X - 20)/40, (block.BodyRectangle.Y - 40)/36);

                blocks[(int) vector.X, (int) vector.Y] = block;
            }

            Debug.WriteLine("-----------------------");
            for (int i = 0; i < 11; i++)
            {
                string line = string.Empty;

                for (int j = 0; j < 15; j++)
                {
                    if (blocks[j, i] == null)
                    {
                        line = line + ".";
                    }
                    else
                    {
                        line = line + "X";
                    }
                }
                Debug.WriteLine(line);
            }
            Debug.WriteLine("-----------------------");

            return blocks;
        }
    }
}