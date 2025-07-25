To run the dotnet project please use below commands in your ide or terminal

Bugs:
1. In Jwt token validation we are simply checking the signature, subject, expiration, 
but not user credentials (In Spring boot we are doing something that I want to replicate in the dotnet too)
2. Delete dependencies when deleting a entity in database like deleting the 
3. delivery hub address before deleting hub
4. Handle the sql level exceptions in repository layer (Ex: if I miss any non null column in db and 
it return the exception with 500 status code directly to end user) and even mappings failures also
5. Globally handle the exceptions.
6. Use Scope (Transaction scope for rolling if any error occurs)
7. In spring boot, services should be configured such that when a jwt token expires, 
it should send same response like "Token expired" instead of unauthorized message and 401 status code

Todo:
1. Check if same orderItemId already has a return or not before creating the orderReturn
2. OrderReturns CRUD Api's
3. OrderTracking CRUD Api's
4. Should notify the user when order status is changed.
5. should update the order_details when order tracking changes its status
6. In Address controller, while adding a new address, one has to verify the userId is valid or not. 
7. Update Delivery Agent Password API pending 
8. Use loggers


Where I Stopped ?

completed the concrete estimation of time for delivery or return pickup
I didn't registered the services, repositories of order returns and order tracking.
Need to work on AssignNearestDeliveryAgent method
Need to test the google matrix api



