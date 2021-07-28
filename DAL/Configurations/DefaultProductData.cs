using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;

using WebAppLab.Data.Constants;
using WebAppLab.Data.Models;

namespace WebAppLab.Data.Configurations
{
	public class DefaultProductData : IEntityTypeConfiguration<Product>
    {
        private readonly Product[] products =
        {
            new()
            {
                Id = Guid.Parse("90FAA83B-0427-44B5-8D35-5F81BBBAD1C4"),
                Name = "Biomutant",
                Platform = Platforms.PlayStation4,
                DateCreated = DateTime.Parse("May 25, 2021"),
                TotalRating = 64
            },
            new()
            {
                Id = Guid.Parse("1663CFEF-1671-4216-839D-8DC119B27BF2"),
                Name = "NieR Replicant ver.1.22474487139",
                Platform = Platforms.XboxOne,
                DateCreated = DateTime.Parse("Apr 23, 2021"),
                TotalRating = 83
            },
            new()
            {
                Id = Guid.Parse("F1B8089D-5DD7-4C37-8053-DDEC627EC63D"),
                Name = "Monster Hunter Stories 2: Wings of Ruin",
                Platform = Platforms.Switch,
                DateCreated = DateTime.Parse("Jul 9, 2021"),
                TotalRating = 81
            },
            new()
            {
                Id = Guid.Parse("237C3BA6-5FDC-4644-AD9B-1AB8A629EDFA"),
                Name = "Scarlet Nexus",
                Platform = Platforms.XboxSX,
                DateCreated = DateTime.Parse("Jun 25, 2021"),
                TotalRating = 77
            },
            new()
            {
                Id = Guid.Parse("0260FF54-243E-4CF1-8332-77976D00C3BC"),
                Name = "Dungeons & Dragons: Dark Alliance",
                Platform = Platforms.PlayStation5,
                DateCreated = DateTime.Parse("Jun 22, 2021"),
                TotalRating = 58
            },
            new()
            {
                Id = Guid.Parse("CF6B66BC-A9D2-4AEC-A9C6-E8322C079E03"),
                Name = "Mass Effect Legendary Edition",
                Platform = Platforms.XboxOne,
                DateCreated = DateTime.Parse("May 14, 2021"),
                TotalRating = 90
            },
            new()
            {
                Id = Guid.Parse("44E2E106-5A13-4863-8326-A01E340883E4"),
                Name = "Final Fantasy VII Remake Intergrade",
                Platform = Platforms.PlayStation5,
                DateCreated = DateTime.Parse("Jun 10, 2021"),
                TotalRating = 89
            },
            new()
            {
                Id = Guid.Parse("C99A81F9-2056-41A3-A968-63238637B3F3"),
                Name = "Green Hell",
                Platform = Platforms.XboxOne,
                DateCreated = DateTime.Parse("Jun 9, 2021"),
                TotalRating = 85
            },
            new()
            {
                Id = Guid.Parse("CF64F991-31F1-4FA2-8DB6-F274BC8640D5"),
                Name = "Guilty Gear -Strive-",
                Platform = Platforms.PlayStation4,
                DateCreated = DateTime.Parse("Jun 8, 2021"),
                TotalRating = 83
            },
            new()
            {
                Id = Guid.Parse("659D2773-00F2-4D23-9208-7DAB6F7ECCFA"),
                Name = "ScourgeBringer",
                Platform = Platforms.PlayStation4,
                DateCreated = DateTime.Parse("Apr 22, 2021"),
                TotalRating = 81
            },
            new()
            {
                Id = Guid.Parse("64D67D60-9B65-415B-B826-942BDFB01605"),
                Name = "Red Dead Redemption 2",
                Platform = Platforms.PlayStation4,
                DateCreated = DateTime.Parse("Oct 26, 2018"),
                TotalRating = 97
            },
            new()
            {
                Id = Guid.Parse("19F1FA2F-BBA6-4D87-A727-6187E02B2644"),
                Name = "Persona 5 Royal",
                Platform = Platforms.PlayStation4,
                DateCreated = DateTime.Parse("Mar 31, 2020"),
                TotalRating = 95
            },
            new()
            {
                Id = Guid.Parse("36E9CEA7-27F6-4F85-A078-156E7817A75A"),
                Name = "Chicory: A Colorful Tale",
                Platform = Platforms.PC,
                DateCreated = DateTime.Parse("Jun 10, 2021"),
                TotalRating = 89
            },
            new()
            {
                Id = Guid.Parse("C29337A4-CEBB-47FD-B9A3-3F60E16468A0"),
                Name = "Wildermyth",
                Platform = Platforms.PC,
                DateCreated = DateTime.Parse("Jun 15, 2021"),
                TotalRating = 89
            },
            new()
            {
                Id = Guid.Parse("752B8B53-C787-4A8E-84F8-A74FAFC04E96"),
                Name = "Lacuna - A Sci-Fi Noir Adventure",
                Platform = Platforms.PC,
                DateCreated = DateTime.Parse("May 20, 2021"),
                TotalRating = 88
            },
            new()
            {
                Id = Guid.Parse("CDC22AB5-ABF1-4C20-8BD6-3220C0C4D7A6"),
                Name = "Astalon: Tears of the Earth",
                Platform = Platforms.PC,
                DateCreated = DateTime.Parse("June 3, 2021"),
                TotalRating = 87
            },
            new()
            {
                Id = Guid.Parse("19DD47B4-8F29-4A27-8B3C-113D799C7B08"),
                Name = "F1 2021",
                Platform = Platforms.PC,
                DateCreated = DateTime.Parse("July 16, 2021"),
                TotalRating = 86
            },
            new()
            {
                Id = Guid.Parse("446C1DE7-0278-445C-A292-1F3A5A6AE4C3"),
                Name = "Metro Exodus: Complete Edition",
                Platform = Platforms.PC,
                DateCreated = DateTime.Parse("May 6, 2021"),
                TotalRating = 85
            },
        };

		public void Configure(EntityTypeBuilder<Product> builder)
		{
            builder.HasData(products);
		}
	}
}
