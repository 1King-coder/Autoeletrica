namespace InterfacesPlugin
{
    partial class SendingCircuitsToSheets
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.GBselecaoQD = new System.Windows.Forms.GroupBox();
            this.SelectPanelBtn = new System.Windows.Forms.Button();
            this.PanelSelectLbl = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Descricao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumeroCirc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tensao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Esquema = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Potencia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GBdadosPlanilha = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LinkPlanilhaTxt = new System.Windows.Forms.TextBox();
            this.ViewPlanilha = new System.Windows.Forms.WebBrowser();
            this.SendDadosCircBtn = new System.Windows.Forms.Button();
            this.GBselecaoQD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.GBdadosPlanilha.SuspendLayout();
            this.SuspendLayout();
            // 
            // GBselecaoQD
            // 
            this.GBselecaoQD.BackColor = System.Drawing.SystemColors.Control;
            this.GBselecaoQD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.GBselecaoQD.Controls.Add(this.dataGridView1);
            this.GBselecaoQD.Controls.Add(this.PanelSelectLbl);
            this.GBselecaoQD.Controls.Add(this.SelectPanelBtn);
            this.GBselecaoQD.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.GBselecaoQD.ForeColor = System.Drawing.SystemColors.InfoText;
            this.GBselecaoQD.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.GBselecaoQD.Location = new System.Drawing.Point(12, 12);
            this.GBselecaoQD.Name = "GBselecaoQD";
            this.GBselecaoQD.Size = new System.Drawing.Size(612, 271);
            this.GBselecaoQD.TabIndex = 0;
            this.GBselecaoQD.TabStop = false;
            this.GBselecaoQD.Text = "Seleção do Quadro de Distribuição";
            // 
            // SelectPanelBtn
            // 
            this.SelectPanelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.SelectPanelBtn.Location = new System.Drawing.Point(473, 29);
            this.SelectPanelBtn.Name = "SelectPanelBtn";
            this.SelectPanelBtn.Size = new System.Drawing.Size(103, 34);
            this.SelectPanelBtn.TabIndex = 0;
            this.SelectPanelBtn.Text = "Selecionar";
            this.SelectPanelBtn.UseVisualStyleBackColor = true;
            // 
            // PanelSelectLbl
            // 
            this.PanelSelectLbl.AutoSize = true;
            this.PanelSelectLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.PanelSelectLbl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PanelSelectLbl.Location = new System.Drawing.Point(159, 36);
            this.PanelSelectLbl.Name = "PanelSelectLbl";
            this.PanelSelectLbl.Size = new System.Drawing.Size(268, 20);
            this.PanelSelectLbl.TabIndex = 1;
            this.PanelSelectLbl.Text = "Selecione um Quadro de distribuição";
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Descricao,
            this.NumeroCirc,
            this.Tensao,
            this.Esquema,
            this.Potencia});
            this.dataGridView1.Location = new System.Drawing.Point(10, 80);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(596, 165);
            this.dataGridView1.TabIndex = 2;
            // 
            // Descricao
            // 
            this.Descricao.HeaderText = "Descrição";
            this.Descricao.Name = "Descricao";
            this.Descricao.ReadOnly = true;
            // 
            // NumeroCirc
            // 
            this.NumeroCirc.HeaderText = "Circuito";
            this.NumeroCirc.Name = "NumeroCirc";
            this.NumeroCirc.ReadOnly = true;
            // 
            // Tensao
            // 
            this.Tensao.HeaderText = "Tensão";
            this.Tensao.Name = "Tensao";
            this.Tensao.ReadOnly = true;
            // 
            // Esquema
            // 
            this.Esquema.HeaderText = "Esquema";
            this.Esquema.Name = "Esquema";
            this.Esquema.ReadOnly = true;
            // 
            // Potencia
            // 
            this.Potencia.HeaderText = "Potência";
            this.Potencia.Name = "Potencia";
            this.Potencia.ReadOnly = true;
            // 
            // GBdadosPlanilha
            // 
            this.GBdadosPlanilha.Controls.Add(this.ViewPlanilha);
            this.GBdadosPlanilha.Controls.Add(this.LinkPlanilhaTxt);
            this.GBdadosPlanilha.Controls.Add(this.label1);
            this.GBdadosPlanilha.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.GBdadosPlanilha.ForeColor = System.Drawing.SystemColors.InfoText;
            this.GBdadosPlanilha.Location = new System.Drawing.Point(12, 289);
            this.GBdadosPlanilha.Name = "GBdadosPlanilha";
            this.GBdadosPlanilha.Size = new System.Drawing.Size(605, 248);
            this.GBdadosPlanilha.TabIndex = 1;
            this.GBdadosPlanilha.TabStop = false;
            this.GBdadosPlanilha.Text = "Dados Planilha de dimensionamento";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(21, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Link Planilha de Dimensionamento:";
            // 
            // LinkPlanilhaTxt
            // 
            this.LinkPlanilhaTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.LinkPlanilhaTxt.Location = new System.Drawing.Point(256, 37);
            this.LinkPlanilhaTxt.Name = "LinkPlanilhaTxt";
            this.LinkPlanilhaTxt.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LinkPlanilhaTxt.Size = new System.Drawing.Size(332, 21);
            this.LinkPlanilhaTxt.TabIndex = 1;
            // 
            // ViewPlanilha
            // 
            this.ViewPlanilha.Location = new System.Drawing.Point(23, 65);
            this.ViewPlanilha.MinimumSize = new System.Drawing.Size(20, 20);
            this.ViewPlanilha.Name = "ViewPlanilha";
            this.ViewPlanilha.Size = new System.Drawing.Size(564, 170);
            this.ViewPlanilha.TabIndex = 2;
            // 
            // SendDadosCircBtn
            // 
            this.SendDadosCircBtn.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SendDadosCircBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SendDadosCircBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.SendDadosCircBtn.Location = new System.Drawing.Point(147, 543);
            this.SendDadosCircBtn.Name = "SendDadosCircBtn";
            this.SendDadosCircBtn.Size = new System.Drawing.Size(341, 73);
            this.SendDadosCircBtn.TabIndex = 2;
            this.SendDadosCircBtn.Text = "Enviar";
            this.SendDadosCircBtn.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 647);
            this.Controls.Add(this.SendDadosCircBtn);
            this.Controls.Add(this.GBdadosPlanilha);
            this.Controls.Add(this.GBselecaoQD);
            this.Name = "Form1";
            this.Text = "Form1";
            this.GBselecaoQD.ResumeLayout(false);
            this.GBselecaoQD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.GBdadosPlanilha.ResumeLayout(false);
            this.GBdadosPlanilha.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GBselecaoQD;
        private System.Windows.Forms.Label PanelSelectLbl;
        private System.Windows.Forms.Button SelectPanelBtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descricao;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumeroCirc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tensao;
        private System.Windows.Forms.DataGridViewTextBoxColumn Esquema;
        private System.Windows.Forms.DataGridViewTextBoxColumn Potencia;
        private System.Windows.Forms.GroupBox GBdadosPlanilha;
        private System.Windows.Forms.WebBrowser ViewPlanilha;
        private System.Windows.Forms.TextBox LinkPlanilhaTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SendDadosCircBtn;
    }
}

