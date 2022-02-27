using Microsoft.AspNetCore.Identity;

namespace ExampleApi.Data.Entities.Authority
{
	////because we inherit from IdentityUser, we get all the properties with it as well as the ones we place here
	////to store the identities, in this example, we will be using the default implementation in Identity. DbContext are needs to implement as IdentityDbContext
	////aslo, in this example, we need to tie the Entity peice in to the order system by going into Order.cs and adding the 'public StoreUser User { get; set; }' property
	//public class AuthorityUser : IdentityUser  //IdentityUser is a user that knows what kind of identity a user is.
	//{
	//	public string FirstName { get; set; }
	//	public string LastName { get; set; }
	//}
}
