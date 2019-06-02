# customer-segmentation
A windows-form application to do customer segmentation base on Vector Quantization Algorithm. Total purchases of customers will be the parameter to decided the segmentation.

# To Test the application 
1. Open XAMPP then start Apache and Mysql
2. Go to browser and go with localhost/phpmyadmin
3. Create database named pos
4. Import the sample database
5. Install VQ.exe and run the application
6. Go to tab Transaksi
7. Hit button Proses
8. And you will get the list of customer segmentation

# About the application

In this application, I just categorize 3 type of customer (Regular, Potential and VIP) but I believe it can be developed into more than 3 types. Customers who have the highest total purchases will be grouped into VIPs. Customers who have the lowest total purchases will be grouped into Regulars. And the ones who have the total purchases between highest and lowest will be Potential. At this point, the vector quantization will decided which value is high/low/medium. For detail explanation on how the application works, please go to file explanation-with-sample-case.docx
