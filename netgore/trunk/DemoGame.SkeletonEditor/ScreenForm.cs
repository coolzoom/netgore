using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetGore;
using NetGore.Graphics;
using Color=System.Drawing.Color;

namespace DemoGame.SkeletonEditor
{
    public partial class ScreenForm : Form
    {
        const string _filterBody = "Skeleton body (*.skelb)|*.skelb";
        const string _filterFrame = "Skeleton frame (*.skel)|*.skel";
        const string _filterSet = "Skeleton set (*.skels)|*.skels";
        static readonly Color ColorError = Color.Red;
        static readonly Color ColorNormal = SystemColors.Window;

        readonly List<XNALine> _centerLines = new List<XNALine>();
        readonly Stopwatch _watch = new Stopwatch();

        Camera2D _camera = new Camera2D(GameData.ScreenSize);
        ContentManager _content;
        int _currentTime = 0;

        /// <summary>
        /// World position of the cursor for the game screen
        /// </summary>
        Vector2 _cursorPos = new Vector2();

        string _fileAnim = string.Empty;
        string _fileBody = string.Empty;
        string _fileFrame = string.Empty;
        SkeletonBody _frameBody = null;
        bool _moveSelectedNode = false;
        SpriteBatch _sb;
        SkeletonNode _selectedNode = null;
        Skeleton _skeleton;
        SkeletonAnimation _skeletonAnim;
        SkeletonDrawer _skeletonDrawer;
        KeyEventArgs ks = new KeyEventArgs(Keys.None);

        /// <summary>
        /// Gets or sets the file for the current skeleton animation
        /// </summary>
        string FileAnim
        {
            get { return _fileAnim; }
            set
            {
                _fileAnim = value;
                lblAnimation.Text = "Loaded: " + Path.GetFileName(_fileAnim);
            }
        }

        /// <summary>
        /// Gets or sets the file for the current skeleton body
        /// </summary>
        string FileBody
        {
            get { return _fileBody; }
            set
            {
                _fileBody = value;
                gbBodies.Text = "Bone Bodies: " + Path.GetFileName(_fileBody);
            }
        }

        /// <summary>
        /// Gets or sets the file for the current skeleton frame
        /// </summary>
        string FileFrame
        {
            get { return _fileFrame; }
            set
            {
                _fileFrame = value;
                lblSkeleton.Text = "Loaded: " + Path.GetFileName(_fileFrame);
            }
        }

        /// <summary>
        /// Gets the selected drawable skeleton item in the list
        /// </summary>
        public SkeletonBodyItem SelectedDSI
        {
            get
            {
                try
                {
                    return ((ListItem<SkeletonBodyItem>)lstBodies.SelectedItem).Item;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected node
        /// </summary>
        public SkeletonNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (_selectedNode != value)
                {
                    _selectedNode = value;
                    ChangeSelectedNode();
                }
            }
        }

        /// <summary>
        /// Gets the SkeletonAnimation's SkeletonBody
        /// </summary>
        public SkeletonBody SkeletonBody
        {
            get { return _skeletonAnim.SkeletonBody; }
        }

        public ScreenForm()
        {
            InitializeComponent();
            HookInput();
            GameScreen.Parent = this;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            Array.Resize(ref SkeletonBody.BodyItems, SkeletonBody.BodyItems.Length + 1);
            SkeletonBody.BodyItems[SkeletonBody.BodyItems.Length - 1] =
                new SkeletonBodyItem(new SkeletonBodyItemInfo(0, _skeleton.RootNode.Name, string.Empty, Vector2.Zero, Vector2.Zero));
            UpdateBodyList();
        }

        void btnAnimLoad_Click(object sender, EventArgs e)
        {
            string result = GetLoadSkeletonDialogResult(_filterSet);

            if (result != null && result.Length > 1)
                LoadAnim(result);
        }

        void btnAnimSave_Click(object sender, EventArgs e)
        {
            if (FileAnim != null && FileAnim.Length > 1)
                SkeletonSet.Save(SkeletonSet.Load(txtFrames.Text, "\r\n"), FileAnim);
        }

        void btnAnimSaveAs_Click(object sender, EventArgs e)
        {
            string result = GetSaveSkeletonDialogResult(_filterSet);

            if (result != null && result.Length > 1)
            {
                SkeletonSet.Save(SkeletonSet.Load(txtFrames.Text, "\r\n"), result);
                FileAnim = result;
            }
        }

        void btnBodyLoad_Click(object sender, EventArgs e)
        {
            string result = GetLoadSkeletonDialogResult(_filterBody);

            if (result != null && result.Length > 1)
                LoadBody(result);
        }

        void btnBodySave_Click(object sender, EventArgs e)
        {
            if (FileBody != null && FileBody.Length > 1)
                SkeletonBody.BodyInfo.Save(FileBody);
        }

        void btnBodySaveAs_Click(object sender, EventArgs e)
        {
            string result = GetSaveSkeletonDialogResult(_filterBody);

            if (result != null && result.Length > 1)
            {
                SkeletonBody.BodyInfo.Save(result);
                FileBody = result;
            }
        }

        void btnCopyInherits_Click(object sender, EventArgs e)
        {
            var results = GetLoadSkeletonDialogResults(_filterFrame);

            if (results == null || results.Length <= 0)
                return;

            foreach (string s in results)
            {
                if (!File.Exists(s))
                    continue;

                Skeleton tmpSkel = Skeleton.Load(s);
                Skeleton.CopyIsModifier(_skeleton, tmpSkel);
                tmpSkel.Save(s);
            }
        }

        void btnCopyLen_Click(object sender, EventArgs e)
        {
            var results = GetLoadSkeletonDialogResults(_filterFrame);

            if (results == null || results.Length <= 0)
                return;

            foreach (string s in results)
            {
                if (!File.Exists(s))
                    continue;

                Skeleton tmpSkel = Skeleton.Load(s);
                Skeleton.CopyLength(_skeleton, tmpSkel);
                tmpSkel.Save(s);
            }
        }

        void btnCopyRoot_Click(object sender, EventArgs e)
        {
            DialogResult rX = MessageBox.Show("Copy the X axis?", "Skeleton frame root copy", MessageBoxButtons.YesNoCancel);
            if (rX == DialogResult.Cancel)
                return;

            DialogResult rY = MessageBox.Show("Copy the Y axis?", "Skeleton frame root copy", MessageBoxButtons.YesNoCancel);
            if (rY == DialogResult.Cancel)
                return;

            if (rX == DialogResult.No && rY == DialogResult.No)
                return;

            var results = GetLoadSkeletonDialogResults(_filterFrame);

            if (results != null && results.Length > 0)
            {
                foreach (string s in results)
                {
                    if (!File.Exists(s))
                        continue;

                    Skeleton tmpSkel = Skeleton.Load(s);
                    Vector2 newPos = _skeleton.RootNode.Position;
                    if (rX == DialogResult.Yes)
                        newPos.X = tmpSkel.RootNode.X;
                    if (rY == DialogResult.Yes)
                        newPos.Y = tmpSkel.RootNode.Y;
                    tmpSkel.RootNode.MoveTo(newPos);
                    Skeleton.Save(tmpSkel, s);
                }
            }
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstBodies.SelectedIndex == -1)
                return;

            int sel = lstBodies.SelectedIndex;
            for (int i = sel; i < lstBodies.Items.Count - 1; i++)
            {
                lstBodies.Items[i] = lstBodies.Items[i + 1];
                SkeletonBody.BodyItems[i] = SkeletonBody.BodyItems[i + 1];
            }
            lstBodies.Items.RemoveAt(lstBodies.Items.Count - 1);
            Array.Resize(ref SkeletonBody.BodyItems, SkeletonBody.BodyItems.Length - 1);
        }

        void btnDown_Click(object sender, EventArgs e)
        {
            int selIndex = lstBodies.SelectedIndex;
            if (selIndex < 0 || selIndex >= lstBodies.Items.Count - 1)
                return;

            object o1 = lstBodies.Items[selIndex];
            object o2 = lstBodies.Items[selIndex + 1];
            lstBodies.Items[selIndex] = o2;
            lstBodies.Items[selIndex + 1] = o1;

            o1 = SkeletonBody.BodyItems[selIndex];
            o2 = SkeletonBody.BodyItems[selIndex + 1];
            SkeletonBody.BodyItems[selIndex] = (SkeletonBodyItem)o2;
            SkeletonBody.BodyItems[selIndex + 1] = (SkeletonBodyItem)o1;

            lstBodies.SelectedIndex = selIndex + 1;
        }

        void btnFall_Click(object sender, EventArgs e)
        {
            if (!radioAnimate.Checked)
                return;

            SkeletonSet newSet = SkeletonSet.Load(ContentPaths.Dev.Skeletons.Join("fall.skels"));
            _skeletonAnim.ChangeSet(newSet);
        }

        void btnInterpolate_Click(object sender, EventArgs e)
        {
            string result = GetLoadSkeletonDialogResult(_filterFrame);

            SkeletonFrame frame1;
            if (result != null && result.Length > 1)
                frame1 = new SkeletonFrame(result, Skeleton.Load(result), 10);
            else
                return;

            result = GetLoadSkeletonDialogResult(_filterFrame);

            SkeletonFrame frame2;
            if (result != null && result.Length > 1)
                frame2 = new SkeletonFrame(result, Skeleton.Load(result), 10);
            else
                return;

            SkeletonSet ss = new SkeletonSet(new[] { frame1, frame2 });
            SkeletonAnimation sa = new SkeletonAnimation(GetTime(), ss);
            sa.Update(5);
            LoadFrame(sa.Skeleton);
        }

        void btnJump_Click(object sender, EventArgs e)
        {
            if (!radioAnimate.Checked)
                return;

            SkeletonSet newSet = SkeletonSet.Load(ContentPaths.Dev.Skeletons.Join("jump.skels"));
            _skeletonAnim.ChangeSet(newSet);
        }

        void btnPause_Click(object sender, EventArgs e)
        {
            if (_watch.IsRunning)
                _watch.Stop();
            else
                _watch.Start();
        }

        void btnPlay_Click(object sender, EventArgs e)
        {
            if (!_watch.IsRunning)
                _watch.Start();

            SetAnimByTxt();
        }

        static void btnShiftNodes_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make this button shift all nodes in all seleced files by a defined amount");
        }

        void btnSkeletonLoad_Click(object sender, EventArgs e)
        {
            string result = GetLoadSkeletonDialogResult(_filterFrame);

            if (result != null && result.Length > 1)
                LoadFrame(result);
        }

        void btnSkeletonSave_Click(object sender, EventArgs e)
        {
            if (FileFrame != null && FileFrame.Length > 1)
                Skeleton.Save(_skeleton, FileFrame);
        }

        void btnSkeletonSaveAs_Click(object sender, EventArgs e)
        {
            string result = GetSaveSkeletonDialogResult(_filterFrame);

            if (result != null && result.Length > 1)
            {
                Skeleton.Save(_skeleton, result);
                FileFrame = result;
            }
        }

        void btnStand_Click(object sender, EventArgs e)
        {
            if (!radioAnimate.Checked)
                return;

            Skeleton newSkeleton = Skeleton.Load(ContentPaths.Dev.Skeletons.Join("stand.skel"));
            SkeletonFrame nFrame0 = new SkeletonFrame("stand", newSkeleton);
            _skeletonAnim.ChangeSet(new SkeletonSet(new[] { nFrame0 }));
        }

        void btnUp_Click(object sender, EventArgs e)
        {
            int selIndex = lstBodies.SelectedIndex;
            if (selIndex <= 0)
                return;

            object o1 = lstBodies.Items[selIndex];
            object o2 = lstBodies.Items[selIndex - 1];
            lstBodies.Items[selIndex] = o2;
            lstBodies.Items[selIndex - 1] = o1;

            o1 = SkeletonBody.BodyItems[selIndex];
            o2 = SkeletonBody.BodyItems[selIndex - 1];
            SkeletonBody.BodyItems[selIndex] = (SkeletonBodyItem)o2;
            SkeletonBody.BodyItems[selIndex - 1] = (SkeletonBodyItem)o1;

            lstBodies.SelectedIndex = selIndex - 1;
        }

        void btnWalk_Click(object sender, EventArgs e)
        {
            if (!radioAnimate.Checked)
                return;

            SkeletonSet newSet = SkeletonSet.Load(ContentPaths.Dev.Skeletons.Join("walk.skels"));
            _skeletonAnim.ChangeSet(newSet);
        }

        void ChangeSelectedNode()
        {
            foreach (object item in cmbSkeletonNodes.Items)
            {
                if (((ListItem<SkeletonNode>)item).Item == SelectedNode)
                {
                    cmbSkeletonNodes.SelectedItem = item;
                    break;
                }
            }
            UpdateNodeInfo();
        }

        void chkIsMod_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedNode.IsModifier = chkIsMod.Checked;
                chkIsMod.BackColor = ColorNormal;
            }
            catch
            {
                chkIsMod.BackColor = ColorError;
            }
        }

        void cmbSkeletonNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedNode = ((ListItem<SkeletonNode>)cmbSkeletonNodes.SelectedItem).Item;
        }

        void cmbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedDSI.Source = ((ListItem<SkeletonNode>)cmbSource.SelectedItem).Item;
                cmbSource.BackColor = ColorNormal;
                UpdateSelectedDSI();
            }
            catch
            {
                cmbSource.BackColor = ColorError;
            }
        }

        void cmbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedDSI.Dest = ((ListItem<SkeletonNode>)cmbTarget.SelectedItem).Item;
                cmbTarget.BackColor = ColorNormal;
                UpdateSelectedDSI();
            }
            catch
            {
                cmbTarget.BackColor = ColorError;
            }
        }

        public void DrawGame()
        {
            GameScreen.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.CornflowerBlue);

            // Screen
            _sb.BeginUnfiltered(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, _camera.Matrix);

            foreach (XNALine l in _centerLines)
            {
                l.Draw(_sb);
            }

            if (radioEdit.Checked)
            {
                // Edit skeleton
                _skeletonDrawer.Draw(_skeleton, _camera, _sb, SelectedNode);
                if (_frameBody != null && chkDrawBody.Checked)
                    _frameBody.Draw(_sb, Vector2.Zero);
            }
            else
            {
                // Animate skeletons
                if (chkDrawBody.Checked)
                    _skeletonAnim.Draw(_sb);
                if (chkDrawSkel.Checked)
                    _skeletonDrawer.Draw(_skeletonAnim.Skeleton, _camera, _sb);
            }
            _sb.End();
        }

        void GameScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (radioEdit.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Select new node
                    var nodes = _skeleton.RootNode.GetAllNodes();
                    foreach (SkeletonNode node in nodes)
                    {
                        if (node.HitTest(_camera, _cursorPos))
                        {
                            SelectedNode = node; // Select the node
                            _moveSelectedNode = true; // Enable dragging
                            break;
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (_skeleton.RootNode == null)
                    {
                        // Create the root node
                        _skeleton.RootNode = new SkeletonNode(_cursorPos);
                        SelectedNode = _skeleton.RootNode;
                        SelectedNode.Name = "New Root";
                    }
                    else
                    {
                        // Create a child node
                        SelectedNode = new SkeletonNode(SelectedNode, _cursorPos);
                    }
                }
            }
        }

        void GameScreen_MouseMove(object sender, MouseEventArgs e)
        {
            _cursorPos = _camera.ToWorld(e.X, e.Y);
            lblXY.Text = string.Format("({0},{1})", Math.Round(_cursorPos.X, 0), Math.Round(_cursorPos.Y, 0));

            if (_skeleton.RootNode == null || e.Button != MouseButtons.Left || !_moveSelectedNode)
                return;

            if (chkCanTransform.Checked)
            {
                if (ks.Control)
                {
                    // Unlocked movement, move the node and its children
                    SelectedNode.MoveTo(_cursorPos);
                    UpdateNodeInfo();
                }
                else
                {
                    // Unlocked movement, move just the one node
                    SelectedNode.Position = _cursorPos;
                    UpdateNodeInfo();
                }
            }
            else
            {
                if (ks.Control)
                {
                    // Locked movement, the node and all of its children
                    SelectedNode.Rotate(_cursorPos);
                    UpdateNodeInfo();
                }
                else
                {
                    // Locked movement, move the node and its children
                    if (SelectedNode.Parent != null)
                        SelectedNode.SetAngle(_cursorPos);
                    else
                        SelectedNode.MoveTo(_cursorPos);
                    UpdateNodeInfo();
                }
            }
        }

        void GameScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _moveSelectedNode = false;
        }

        static string GetDSIString(SkeletonBodyItem dsi)
        {
            string s = "_null_";
            if (dsi.Dest != null)
                s = dsi.Dest.Name;

            string textureName;
            if (dsi.Grh.GrhData == null)
                textureName = "*";
            else
                textureName = dsi.Grh.GrhData.TextureName.Replace("Character/", string.Empty);

            return textureName + ": " + dsi.Source.Name + " -> " + s;
        }

        static string GetLoadSkeletonDialogResult(string filter)
        {
            string result;

            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = filter;
                fd.InitialDirectory = ContentPaths.Dev.Skeletons;
                fd.RestoreDirectory = true;
                fd.ShowDialog();
                result = fd.FileName;
            }

            return result;
        }

        static string[] GetLoadSkeletonDialogResults(string filter)
        {
            string[] result;

            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = filter;
                fd.InitialDirectory = ContentPaths.Dev.Skeletons;
                fd.RestoreDirectory = true;
                fd.ShowDialog();
                result = fd.FileNames;
            }

            return result;
        }

        static string GetSaveSkeletonDialogResult(string filter)
        {
            string result;

            using (SaveFileDialog fd = new SaveFileDialog())
            {
                fd.Filter = filter;
                fd.InitialDirectory = ContentPaths.Dev.Skeletons;
                fd.RestoreDirectory = true;
                fd.ShowDialog();
                result = fd.FileName;
            }

            return result;
        }

        /// <summary>
        /// Gets the current game time where time 0 is when the application started
        /// </summary>
        /// <returns>Current game time in milliseconds</returns>
        public int GetTime()
        {
            return _currentTime;
        }

        void HookInput()
        {
            RecursiveHookInput(this);
        }

        void KeyDownForward(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        void KeyUpForward(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        public void LoadAnim(string filePath)
        {
            _skeletonAnim.ChangeSet(SkeletonSet.Load(filePath));
            FileAnim = filePath;
            txtFrames.Text = _skeletonAnim.SkeletonSet.GetFramesString();
            UpdateAnimationNodeCBs();
        }

        public void LoadBody(string filePath)
        {
            SkeletonBodyInfo bodyInfo = new SkeletonBodyInfo(filePath);
            _skeletonAnim.SkeletonBody = new SkeletonBody(bodyInfo, _skeletonAnim.Skeleton);
            _frameBody = new SkeletonBody(bodyInfo, _skeleton);
            UpdateBodyList();
            FileBody = filePath;
        }

        public void LoadFrame(string filePath)
        {
            LoadFrame(Skeleton.Load(filePath));
            FileFrame = filePath;
        }

        void LoadFrame(Skeleton skel)
        {
            _skeleton = skel;
            if (_frameBody != null)
                _frameBody.Attach(_skeleton);
            SelectedNode = _skeleton.RootNode;
            UpdateFrameNodeCBs();
        }

        void lstBodies_SelectedIndexChanged(object sender, EventArgs e)
        {
            SkeletonBodyItem item = SelectedDSI;
            if (item == null)
                return;

            txtOffsetX.Text = item.ItemInfo.Offset.X.ToString();
            txtOffsetY.Text = item.ItemInfo.Offset.Y.ToString();
            txtOriginX.Text = item.ItemInfo.Origin.X.ToString();
            txtOriginY.Text = item.ItemInfo.Origin.Y.ToString();
            if (item.Grh.GrhData != null)
                txtGrhIndex.Text = item.Grh.GrhData.GrhIndex.ToString();

            foreach (ListItem<SkeletonNode> node in cmbSource.Items)
            {
                if (node.Item.Name == item.Source.Name)
                    cmbSource.SelectedItem = node;
            }

            if (item.Dest == null)
                cmbTarget.SelectedIndex = 0;
            else
            {
                foreach (ListItem<SkeletonNode> node in cmbTarget.Items)
                {
                    if (node.Item != null && node.Item.Name == item.Dest.Name)
                        cmbTarget.SelectedItem = node;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ks = e;

            const float TranslateRate = 7f;
            const float ScaleRate = 0.08f;

            switch (e.KeyCode)
            {
                case Keys.NumPad8:
                    _camera.Y -= TranslateRate / _camera.Scale;
                    break;
                case Keys.NumPad4:
                    _camera.X -= TranslateRate / _camera.Scale;
                    break;
                case Keys.NumPad6:
                    _camera.X += TranslateRate / _camera.Scale;
                    break;
                case Keys.NumPad2:
                    _camera.Y += TranslateRate / _camera.Scale;
                    break;
                case Keys.NumPad9:
                    _camera.Zoom(_camera.Min + ((GameData.ScreenSize / 2) / _camera.Scale), GameData.ScreenSize,
                                 _camera.Scale + ScaleRate);
                    break;
                case Keys.NumPad7:
                    _camera.Zoom(_camera.Min + ((GameData.ScreenSize / 2) / _camera.Scale), GameData.ScreenSize,
                                 _camera.Scale - ScaleRate);
                    break;
                case Keys.Delete:
                    if (chkCanAlter.Checked && SelectedNode != null)
                    {
                        SkeletonNode removeNode = SelectedNode;
                        SelectedNode = SelectedNode.Parent;
                        removeNode.Remove();
                    }
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            ks = e;
        }

        void radioAnimate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAnimate.Checked)
                SetAnimByTxt();
        }

        void RecursiveHookInput(Control rootC)
        {
            foreach (Control c in rootC.Controls)
            {
                if (c.GetType() != typeof(TextBoxBase) && c.GetType() != typeof(ListControl))
                {
                    c.KeyDown += KeyDownForward;
                    c.KeyUp += KeyUpForward;
                }
                RecursiveHookInput(c);
            }
        }

        void ScreenForm_Load(object sender, EventArgs e)
        {
            // Create the engine objects
            _content = new ContentManager(GameScreen.Services, "Content");
            _sb = new SpriteBatch(GameScreen.GraphicsDevice);
            _camera = new Camera2D(GameData.ScreenSize);
            GrhInfo.Load(ContentPaths.Dev.Data.Join("grhdata.xml"), _content);

            // Create the skeleton-related objects
            _skeleton = new Skeleton();
            SkeletonFrame frame = new SkeletonFrame("stand", Skeleton.Load(ContentPaths.Dev.Skeletons.Join("stand.skel")));
            _skeletonAnim = new SkeletonAnimation(GetTime(), frame);
            _skeletonDrawer = new SkeletonDrawer();

            _sb = new SpriteBatch(GameScreen.GraphicsDevice);

            _camera.Min = new Vector2(-400, -400);
            LoadFrame(ContentPaths.Dev.Skeletons.Join("stand.skel"));
            LoadAnim(ContentPaths.Dev.Skeletons.Join("walk.skels"));
            LoadBody(ContentPaths.Dev.Skeletons.Join("basic.skelb"));

            // Center lines
            _centerLines.Add(new XNALine(new Vector2(-100, 0), new Vector2(100, 0), Microsoft.Xna.Framework.Graphics.Color.Lime));
            _centerLines.Add(new XNALine(new Vector2(0, -5), new Vector2(0, 5), Microsoft.Xna.Framework.Graphics.Color.Red));

            _watch.Start();
        }

        void SetAnimByTxt()
        {
            try
            {
                SkeletonSet newSet = SkeletonSet.Load(txtFrames.Text, "\r\n");
                if (newSet == null)
                    throw new Exception();
                _skeletonAnim.ChangeSet(newSet);
                txtFrames.BackColor = ColorNormal;
            }
            catch
            {
                txtFrames.BackColor = ColorError;
            }
        }

        void txtAngle_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedNode.SetAngle(float.Parse(txtAngle.Text));
                txtAngle.BackColor = ColorNormal;
            }
            catch
            {
                txtAngle.BackColor = ColorError;
            }
        }

        void txtFrames_TextChanged(object sender, EventArgs e)
        {
            SetAnimByTxt();
        }

        void txtGrhIndex_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ushort grhIndex = ushort.Parse(txtGrhIndex.Text);
                SelectedDSI.Grh = new Grh(grhIndex, AnimType.Loop, (int)_watch.ElapsedMilliseconds);
                txtGrhIndex.BackColor = ColorNormal;
                UpdateSelectedDSI();
            }
            catch
            {
                txtGrhIndex.BackColor = ColorError;
            }
        }

        void txtLength_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedNode.SetLength(float.Parse(txtLength.Text));
                txtLength.BackColor = ColorNormal;
            }
            catch
            {
                txtLength.BackColor = ColorError;
            }
        }

        void txtName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedNode.Name = txtName.Text;
                var item = (ListItem<SkeletonNode>)cmbSkeletonNodes.Items[cmbSkeletonNodes.SelectedIndex];
                item.Name = SelectedNode.Name;
                cmbSkeletonNodes.Items[cmbSkeletonNodes.SelectedIndex] = item;
                txtName.BackColor = ColorNormal;
            }
            catch
            {
                txtName.BackColor = ColorError;
            }
        }

        void txtOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedDSI.ItemInfo.Offset = new Vector2(float.Parse(txtOffsetX.Text), SelectedDSI.ItemInfo.Offset.Y);
                txtOffsetX.BackColor = ColorNormal;
            }
            catch
            {
                txtOffsetX.BackColor = ColorError;
            }
        }

        void txtOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedDSI.ItemInfo.Offset = new Vector2(SelectedDSI.ItemInfo.Offset.X, float.Parse(txtOffsetY.Text));
                txtOffsetY.BackColor = ColorNormal;
            }
            catch
            {
                txtOffsetY.BackColor = ColorError;
            }
        }

        void txtOriginX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedDSI.ItemInfo.Origin = new Vector2(float.Parse(txtOriginX.Text), SelectedDSI.ItemInfo.Origin.Y);
                txtOriginX.BackColor = ColorNormal;
            }
            catch
            {
                txtOriginX.BackColor = ColorError;
            }
        }

        void txtOriginY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedDSI.ItemInfo.Origin = new Vector2(SelectedDSI.ItemInfo.Origin.X, float.Parse(txtOriginY.Text));
                txtOriginY.BackColor = ColorNormal;
            }
            catch
            {
                txtOriginY.BackColor = ColorError;
            }
        }

        void txtX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SelectedNode.Parent == null)
                    SelectedNode.MoveTo(new Vector2(float.Parse(txtX.Text), SelectedNode.Y));
                else
                    SelectedNode.X = float.Parse(txtX.Text);
                txtX.BackColor = ColorNormal;
            }
            catch
            {
                txtX.BackColor = ColorError;
            }
        }

        void txtY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SelectedNode.Parent == null)
                    SelectedNode.MoveTo(new Vector2(SelectedNode.X, float.Parse(txtY.Text)));
                else
                    SelectedNode.Y = float.Parse(txtY.Text);
                txtY.BackColor = ColorNormal;
            }
            catch
            {
                txtY.BackColor = ColorError;
            }
        }

        void UpdateAnimationNodeCBs()
        {
            var nodes = _skeletonAnim.Skeleton.RootNode.GetAllNodes();
            cmbSource.Items.Clear();
            cmbTarget.Items.Clear();
            cmbTarget.Items.Add(new ListItem<SkeletonNode>(null, "_null_"));
            foreach (SkeletonNode node in nodes)
            {
                var listItem = new ListItem<SkeletonNode>(node, node.Name);
                cmbSource.Items.Add(listItem);
                cmbTarget.Items.Add(listItem);
            }
        }

        void UpdateBodyList()
        {
            lstBodies.Items.Clear();
            for (int i = 0; i < SkeletonBody.BodyItems.Length; i++)
            {
                SkeletonBodyItem item = SkeletonBody.BodyItems[i];
                lstBodies.Items.Add(new ListItem<SkeletonBodyItem>(item, GetDSIString(item)));
            }
            UpdateAnimationNodeCBs();
        }

        void UpdateFrameNodeCBs()
        {
            cmbSkeletonNodes.Items.Clear();
            var nodes = _skeleton.RootNode.GetAllNodes();
            foreach (SkeletonNode node in nodes)
            {
                cmbSkeletonNodes.Items.Add(new ListItem<SkeletonNode>(node, node.Name));
            }
        }

        public void UpdateGame()
        {
            if (!_watch.IsRunning)
                return;

            _currentTime = (int)_watch.ElapsedMilliseconds;
            _skeletonAnim.Update(_currentTime);
        }

        public void UpdateNodeInfo()
        {
            if (SelectedNode == null)
                return;

            txtName.Text = SelectedNode.Name;
            txtX.Text = SelectedNode.X.ToString();
            txtY.Text = SelectedNode.Y.ToString();
            txtAngle.Text = SelectedNode.GetAngle().ToString();
            txtLength.Text = SelectedNode.GetLength().ToString();
            chkIsMod.Checked = SelectedNode.IsModifier;
        }

        void UpdateSelectedDSI()
        {
            var dsi = (ListItem<SkeletonBodyItem>)lstBodies.Items[lstBodies.SelectedIndex];
            dsi.Name = GetDSIString(dsi.Item);
            lstBodies.Items[lstBodies.SelectedIndex] = dsi;
        }
    }
}