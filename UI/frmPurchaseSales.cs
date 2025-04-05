using DGVPrinterHelper;
using EZ_Bill.BLL;
using EZ_Bill.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Windows.Forms;

namespace EZ_Bill.UI
{
    public partial class frmPurchaseSales : Form
    {
        public frmPurchaseSales()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        DeaCustDAL dcDal = new DeaCustDAL();
        ProductDAL pDal = new ProductDAL();
        DataTable transactiondt = new DataTable();
        userDAL uDAL = new userDAL();
        TransactionsDAL tDAL = new TransactionsDAL();
        TransactionsDetails tdDAL = new TransactionsDetails();

        private void frmPurchaseSales_Load(object sender, EventArgs e)
        {
            string type = frmUserDashboard.transactionType;

            lblTop.Text = type;

            //specify columns for transaction data table
            transactiondt.Columns.Add("Product Name");
            transactiondt.Columns.Add("Rate");
            transactiondt.Columns.Add("Quantity");
            transactiondt.Columns.Add("Total");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearch.Text;

            if(keywords == "")
            {
                txtName.Text = "";
                txtEmail.Text = "";
                txtContact.Text = "";
                txtAddress.Text = "";
                return;
            }

            DeaCustBLL dc = dcDal.SearchDealerCustomerForTransaction(keywords);

            txtName.Text = dc.name;
            txtEmail.Text = dc.email;
            txtContact.Text = dc.contact;
            txtAddress.Text = dc.address;
        }

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            string keywords = txtSearchProduct.Text;

            if(keywords == "")
            {
                txtProductName.Text = "";
                txtProductInventory.Text = "";
                txtProductRate.Text = "";
                txtProductQty.Text = "";
                return;

            }

            ProductBLL p = pDal.GetProductForTransaction(keywords);
            txtProductName.Text = p.name;
            txtProductInventory.Text = p.qty.ToString();
            txtProductRate.Text = p.rate.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string productName = txtProductName.Text;
            decimal rate = decimal.Parse(txtProductRate.Text);
            decimal qty = decimal.Parse(txtProductQty.Text);

            decimal total = rate * qty;

            //display the subtotal in textbox
            decimal subTotal = decimal.Parse(txtSubTotal.Text);
            subTotal = subTotal + total;
            txtSubTotal.Text = subTotal.ToString();

            if(productName == "")
            {
                MessageBox.Show("Select Product First. Try Again !!!");
            }
            else
            {
                transactiondt.Rows.Add(productName,rate,qty,total);
                dgvAddedProducts.DataSource = transactiondt;

                //clear textbox after add product
                txtSearchProduct.Text = "";
                txtProductName.Text = "";
                txtProductInventory.Text = "";
                txtProductRate.Text = "";
                txtProductQty.Text = "";
            }
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            string value = txtDiscount.Text;
             
            if(value == "")
            {
                MessageBox.Show("Please add discount");
 
            }
            else
            {
                decimal subTotal = decimal.Parse(txtSubTotal.Text);
                decimal discount = decimal.Parse(txtDiscount.Text);

                //Calculate the grand total
                decimal grandTotal = ((100-discount)/100) * subTotal;

                txtGrandTotal.Text = grandTotal.ToString();
            }
        }

        private void txtGST_TextChanged(object sender, EventArgs e)
        {
           string check = txtGrandTotal.Text;
            if(check == "")
            {
                MessageBox.Show("Calculate the Discount and set the Grand Total First");

            }
            else
            {
                 
                // Ensure valid input
                if (decimal.TryParse(txtGrandTotal.Text, out decimal previousGT) &&
                    decimal.TryParse(txtGST.Text, out decimal gstRate))
                {
                    // Calculate GST  
                    decimal gstAmount = previousGT * gstRate / 100;
                    decimal grandTotalWithGST = previousGT + gstAmount;

                    // Update the grand total textbox with the correctly calculated value
                    txtGrandTotal.Text = grandTotalWithGST.ToString("0.00"); // Format to 2 decimal places
                }
                else
                {
                    MessageBox.Show("Please enter valid numbers for Grand Total and GST.");
                }

            }
        }

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            decimal grandTotal = decimal.Parse(txtGrandTotal.Text);
            decimal paidAmount = decimal.Parse(txtPaidAmount.Text);

            decimal returnAmount = paidAmount - grandTotal;

            //Display return amount
            txtReturnAmount.Text = returnAmount.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            transactionsBLL transaction = new transactionsBLL();

            transaction.type = lblTop.Text;
            string deacustName = txtName.Text;
            DeaCustBLL dc = dcDal.GetDeaCustIdFromName(deacustName);

            transaction.dea_cust_id = dc.id;
            transaction.grandTotal = Math.Round(decimal.Parse(txtGrandTotal.Text),2);
            transaction.transaction_date = DateTime.Now;
            transaction.tax = decimal.Parse(txtGST.Text);
            transaction.discount = decimal.Parse(txtDiscount.Text);

            string username = frmLogin.loggedIn;
            userBLL u = uDAL.GetIDFromUsername(username);
            transaction.added_by = u.id;
            transaction.transactionDetail = transactiondt;

            bool success = false;

            //code to insert transaction and transaction details
            using(TransactionScope scope = new TransactionScope())
            {
                int transactionID = -1;
                bool w = tDAL.Insert(transaction,out transactionID);

                for(int i = 0; i < transactiondt.Rows.Count; i++)
                {
                    transactionsDetailsBLL transactionDetail = new transactionsDetailsBLL();
                    string ProductName = transactiondt.Rows[i][0].ToString();
                    ProductBLL p = pDal.GetProductIdFromName(ProductName);

                    transactionDetail.product_id = p.id;
                    transactionDetail.rate = decimal.Parse(transactiondt.Rows[i][1].ToString());
                    transactionDetail.qty = decimal.Parse(transactiondt.Rows[i][2].ToString());
                    transactionDetail.total = Math.Round(decimal.Parse(transactiondt.Rows[i][3].ToString()),2);
                    transactionDetail.dea_cust_id = dc.id;
                    transactionDetail.added_date = DateTime.Now;
                    transactionDetail.added_by = u.id;

                    //increase or decrease product quantity based on purchase or sales

                    string transactionType = lblTop.Text;

                    bool x = false;
                    if(transactionType == "Purchase")
                    {
                        //Increase the product
                        x = pDal.IncreaseProduct(transactionDetail.product_id, transactionDetail.qty);

                    }
                    else if(transactionType == "Sales")
                    {
                        //decrease the product
                       x = pDal.DecreaseProduct(transactionDetail.product_id, transactionDetail.qty);

                    }

                

                    //Insert transaction details in database
                    bool y = tdDAL.Insert_TransactionDetails(transactionDetail);
                    success = w && x && y;
                    
                }
                if (success == true)
                {
                    scope.Complete();

                    //CODE TO PRINT BILL

                    // Open Save File Dialog
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "PDF Files|*.pdf";
                        saveFileDialog.Title = "Save PDF File";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                // Create a new PDF document
                                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                                document.Info.Title = "Bill Export";

                                // Create a page in the PDF document
                                PdfSharp.Pdf.PdfPage page = document.AddPage();
                                PdfSharp.Drawing.XGraphics gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);

                                // Define fonts
                                PdfSharp.Drawing.XFont titleFont = new PdfSharp.Drawing.XFont("Verdana", 20);
                                PdfSharp.Drawing.XFont subTitleFont = new PdfSharp.Drawing.XFont("Verdana", 14);
                                PdfSharp.Drawing.XFont contentFont = new PdfSharp.Drawing.XFont("Verdana", 12);

                                // Set initial positions
                                double yPosition = 50;

                                // Add Title: EZ Bill
                                gfx.DrawString("EZ Bill", titleFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XRect(0, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopCenter);
                                yPosition += 30;

                                // Add Location: Devgad
                                gfx.DrawString("Devgad", subTitleFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XRect(0, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopCenter);
                                yPosition += 25;

                                // Add Phone: 9067773821
                                gfx.DrawString("Phone: 9067773821", subTitleFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XRect(0, yPosition, page.Width, page.Height), PdfSharp.Drawing.XStringFormats.TopCenter);
                                yPosition += 40;

                                // Add Customer Details (Below the Phone Number)
                                gfx.DrawString("Customer Details:", subTitleFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));
                                yPosition += 25;

                                gfx.DrawString("Name: " + txtName.Text, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));
                                yPosition += 20;

                                gfx.DrawString("Contact: " + txtContact.Text, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));
                                yPosition += 20;

                                gfx.DrawString("Address: " + txtAddress.Text, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));
                                yPosition += 40;

                                // Add Bill Date in the Top-Right Corner
                                string billDate = dtpBillDate.Value.ToString("dd-MM-yyyy");
                                gfx.DrawString("Date: " + billDate, subTitleFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(page.Width - 150, 50)); // Positioned at the top-right corner

                                yPosition += 40;

                                // Table column headers
                                int tableStartX = 50;
                                int[] columnWidths = { 100, 100, 100, 100 }; // Adjust column widths
                                int tableHeight = 20;

                                // Draw table headers
                                gfx.DrawRectangle(PdfSharp.Drawing.XPens.Black, tableStartX, yPosition, columnWidths.Sum(), tableHeight);
                                int xPosition = tableStartX;
                                for (int i = 0; i < dgvAddedProducts.Columns.Count; i++)
                                {
                                    string columnName = dgvAddedProducts.Columns[i].HeaderText;
                                    gfx.DrawString(columnName, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                        new PdfSharp.Drawing.XRect(xPosition, yPosition, columnWidths[i], tableHeight),
                                        PdfSharp.Drawing.XStringFormats.Center);
                                    gfx.DrawLine(PdfSharp.Drawing.XPens.Black, xPosition, yPosition, xPosition, yPosition + tableHeight);
                                    xPosition += columnWidths[i];
                                }
                                gfx.DrawLine(PdfSharp.Drawing.XPens.Black, xPosition, yPosition, xPosition, yPosition + tableHeight);
                                yPosition += tableHeight;

                                // Add rows to the table
                                for (int row = 0; row < dgvAddedProducts.Rows.Count; row++)
                                {
                                    xPosition = tableStartX;
                                    for (int col = 0; col < dgvAddedProducts.Columns.Count; col++)
                                    {
                                        string cellValue = dgvAddedProducts.Rows[row].Cells[col].Value?.ToString() ?? "";
                                        gfx.DrawString(cellValue, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                            new PdfSharp.Drawing.XRect(xPosition, yPosition, columnWidths[col], tableHeight),
                                            PdfSharp.Drawing.XStringFormats.CenterLeft);
                                        gfx.DrawLine(PdfSharp.Drawing.XPens.Black, xPosition, yPosition, xPosition, yPosition + tableHeight);
                                        xPosition += columnWidths[col];
                                    }
                                    gfx.DrawLine(PdfSharp.Drawing.XPens.Black, xPosition, yPosition, xPosition, yPosition + tableHeight);
                                    yPosition += tableHeight;

                                    // Add a new page if the content exceeds the current page height
                                    if (yPosition > page.Height - 100)
                                    {
                                        gfx.DrawLine(PdfSharp.Drawing.XPens.Black, tableStartX, yPosition, xPosition, yPosition); // Close the last row
                                        page = document.AddPage();
                                        gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
                                        yPosition = 50;
                                    }
                                }
                                // Draw bottom border of the table
                                gfx.DrawLine(PdfSharp.Drawing.XPens.Black, tableStartX, yPosition, tableStartX + columnWidths.Sum(), yPosition);

                                // Add Discount, GST, and Grand Total
                                yPosition += 40;
                                gfx.DrawString("Summary", titleFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XRect(50, yPosition, page.Width - 100, page.Height), PdfSharp.Drawing.XStringFormats.TopLeft);
                                yPosition += 30;

                                gfx.DrawString("Discount: " + txtDiscount.Text, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));
                                yPosition += 20;

                                gfx.DrawString("GST: " + txtGST.Text, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));
                                yPosition += 20;

                                gfx.DrawString("Grand Total: " + txtGrandTotal.Text, contentFont, PdfSharp.Drawing.XBrushes.Black,
                                    new PdfSharp.Drawing.XPoint(50, yPosition));

                                // Save the PDF document
                                document.Save(saveFileDialog.FileName);

                                // Show success message
                                MessageBox.Show("PDF saved successfully at: " + saveFileDialog.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                // Show error message
                                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }






                    MessageBox.Show("Transaction Completed Successfully");

                    //clear the data grid view and textfield
                   dgvAddedProducts.DataSource = null;
                    dgvAddedProducts.Rows.Clear();
                    transactiondt.Rows.Clear(); // UPDATE: Clear transactiondt to remove old data
                    txtSearch.Text = "";
                    txtName.Text = "";
                    txtEmail.Text = "";
                    txtContact.Text = "";
                    txtAddress.Text = "";
                    txtSearchProduct.Text = "";
                    txtProductName.Text = "";
                    txtProductInventory.Text = "";
                    txtProductRate.Text = "";
                    txtProductQty.Text = "";
                    txtSubTotal.Text = "0";
                    txtDiscount.Text = "0";
                    txtGST.Text = "0";
                    txtGrandTotal.Text = "0";
                    txtPaidAmount.Text = "0";
                    txtReturnAmount.Text = "0"; 

                }
                else
                {
                    MessageBox.Show("Transaction Failed");
                }

            }

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
           


        }
    }
}
