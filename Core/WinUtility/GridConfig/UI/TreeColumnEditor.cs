﻿using System;
using System.ComponentModel;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList.Columns;
using XCI.Component;
using XCI.Extension;
using XCI.Helper;

namespace XCI.WinUtility.GridConfig
{
    public partial class TreeColumnEditor : UserControlBase
    {
        /// <summary>
        /// 目标表格
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XCITreeGrid TargetGrid { get; set; }

        /// <summary>
        /// 数据列表格
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected XCIGrid Grid { get { return gridGridColumn; } }

        public TreeColumnEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="grid">目标表格</param>
        public void Initialize(XCITreeGrid grid)
        {
            this.TargetGrid = grid;
            Grid.MainBar = TargetGrid.MainBar;
            InitFiledNameCombo();
            InitData();
        }

        private void InitData()
        {
            Grid.DataSource = TargetGrid.Columns;
        }


        private void gridViewGridColumn_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var obj = Grid.Get(e.FocusedRowHandle);
            if (obj != null)
            {
                propertyGridGridColumn.SetObject(obj);
                propertyGridGridColumn.Enabled = true;
            }
            else
            {
                propertyGridGridColumn.SetObject(null);
                propertyGridGridColumn.Enabled = false;
            }
        }


        /// <summary>
        /// 初始化字段名下拉和字段标题下拉
        /// </summary>
        public void InitFiledNameCombo()
        {
            if (TargetGrid.EntityType == null) return;
            var meta = EntityMetadataFactory.Current.Get(TargetGrid.EntityType.FullName);
            if (meta == null) return;
            comboxFieldName.Items.BeginUpdate();
            comboxFieldName.Items.Clear();
            comboxCaption.Items.BeginUpdate();
            comboxCaption.Items.Clear();
            foreach (EntityField item in meta.EntityFields)
            {
                comboxFieldName.Items.Add(item.FieldName);
                comboxCaption.Items.Add(item.FieldComment);
            }
            comboxFieldName.Items.EndUpdate();
            comboxCaption.Items.EndUpdate();
        }


        private void btnAutoGenerated_Click(object sender, EventArgs e)
        {
            if (TargetGrid.EntityType == null)
            {
                XtraMessageBoxHelper.ShowWarning("没有指定表格的实体类型 EntityType");
                return;
            }
            string typeName = TargetGrid.EntityType.FullName;
            var meta = EntityMetadataFactory.Current.Get(typeName);
            if (meta == null)
            {
                XtraMessageBoxHelper.ShowWarning("没有指定实体元数据");
                return;
            }
            XtraMessageBoxHelper.ShowYesNoAndTips("确定自动生成列吗?", d =>
            {
                TargetGrid.OptionsBehavior.AutoPopulateColumns = false;
                TargetGrid.OptionsBehavior.Editable = true;
                TargetGrid.Columns.Clear();
                
                for (int index = 0; index < meta.EntityFields.Count; index++)
                {
                    EntityField filed = meta.EntityFields[index];
                    TreeListColumn column = TargetGrid.Columns.Add();
                    column.Name = "tree_{0}_{1}".FS(typeName, filed.FieldName);
                    column.FieldName = filed.FieldName;
                    column.Caption = filed.FieldComment;
                    column.VisibleIndex = index + 1;
                    column.OptionsColumn.AllowEdit = false;
                    var dataType = EntityMetadata.GetFastProperty(TargetGrid.EntityType, filed.FieldName).Property.PropertyType;
                    if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
                    {
                        column.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
                        column.Format.FormatString = "yyyy-MM-dd HH:mm:ss";
                    }
                }
                Grid.RefreshData();
                TargetGrid.BestFitColumns();
            });
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            TreeListColumn column = TargetGrid.Columns.Add();
            column.Visible = true;
            column.Name = "tree_{0}".FS(StringHelper.GetGuidString());
            column.OptionsColumn.AllowEdit = false;
            Grid.RefreshData();
            Grid.View.MoveLastVisible();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //XtraMessageBoxHelper.ShowYesNoAndWarning("确定要删除选中的列吗?", d =>
            //{
            TreeListColumn entity = Grid.GetSelected<TreeListColumn>();
                Grid.Delete(entity);
            //});
        }


        private void editSearchBox_EditValueChanged(object sender, EventArgs e)
        {
            Grid.View.ApplyFindFilter(((XCIButtonEdit)sender).Text.Trim());
        }

    }
}
