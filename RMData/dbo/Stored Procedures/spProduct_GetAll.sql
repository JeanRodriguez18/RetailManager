CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT [Id], [ProductName], [Description], [RetailPrice], [QuantityInStock]
	FROM dbo.Product
	order by ProductName;

END
