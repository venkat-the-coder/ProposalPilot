using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Shared.DTOs.Template;

namespace ProposalPilot.Infrastructure.Data.SeedData;

/// <summary>
/// Seeds the database with system proposal templates
/// </summary>
public static class TemplateSeeder
{
    public static async Task SeedTemplatesAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            // Check if templates already exist
            if (await context.ProposalTemplates.AnyAsync(t => t.IsSystemTemplate))
            {
                logger.LogInformation("System templates already exist, skipping seed");
                return;
            }

            logger.LogInformation("Seeding system templates...");

            var templates = GetSystemTemplates();

            await context.ProposalTemplates.AddRangeAsync(templates);
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully seeded {Count} system templates", templates.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding system templates");
            throw;
        }
    }

    private static List<ProposalTemplate> GetSystemTemplates()
    {
        return new List<ProposalTemplate>
        {
            // 1. Web Development Template
            new ProposalTemplate
            {
                Name = "Web Development Project",
                Description = "Complete template for custom website development projects",
                Category = "Web Development",
                Tags = SerializeTags(new List<string> { "web", "development", "website", "custom" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "Thank you for considering us for your web development project. We're excited to help bring your vision to life with a modern, responsive website.",
                    ProblemStatement = "We understand that [CLIENT_NAME] needs a professional web presence that effectively showcases your services and converts visitors into customers.",
                    ProposedSolution = "We'll design and develop a custom website using modern technologies, ensuring it's fast, secure, and optimized for search engines. The site will be fully responsive, working seamlessly across all devices.",
                    Methodology = "Our proven development process includes: 1) Discovery & Planning, 2) Design Mockups & Approval, 3) Development & Testing, 4) Launch & Training.",
                    Deliverables = "- Fully responsive website\n- Content Management System (CMS)\n- SEO optimization\n- Analytics integration\n- 30 days post-launch support",
                    Timeline = "- Week 1-2: Discovery & Design\n- Week 3-5: Development\n- Week 6: Testing & Refinement\n- Week 7: Launch",
                    TeamAndExperience = "Our team has delivered 100+ successful web projects. We specialize in modern frameworks and best practices.",
                    TermsAndConditions = "50% deposit required to begin. Final payment due upon project completion.",
                    CallToAction = "We're ready to start building your website. Let's schedule a kickoff call to discuss next steps!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto
                    {
                        Name = "Starter Website",
                        Description = "Perfect for small businesses",
                        PriceMin = 3000,
                        PriceMax = 5000,
                        Features = new List<string> { "5-page website", "Mobile responsive", "Basic SEO", "Contact form" },
                        Timeline = "4 weeks"
                    },
                    Standard = new PricingTierDto
                    {
                        Name = "Professional Website",
                        Description = "For growing businesses",
                        PriceMin = 7000,
                        PriceMax = 12000,
                        Features = new List<string> { "10-15 pages", "Custom design", "CMS integration", "Advanced SEO", "Blog setup" },
                        Timeline = "6-8 weeks"
                    },
                    Premium = new PricingTierDto
                    {
                        Name = "Enterprise Website",
                        Description = "Full-featured solution",
                        PriceMin = 15000,
                        PriceMax = 30000,
                        Features = new List<string> { "Unlimited pages", "Custom functionality", "E-commerce", "API integrations", "Premium support" },
                        Timeline = "10-12 weeks"
                    }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                ThumbnailUrl = "/templates/web-development.jpg",
                EstimatedTimeMinutes = 15
            },

            // 2. Marketing Campaign Template
            new ProposalTemplate
            {
                Name = "Digital Marketing Campaign",
                Description = "Comprehensive digital marketing strategy and execution",
                Category = "Marketing",
                Tags = SerializeTags(new List<string> { "marketing", "digital", "social media", "ads" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "We're thrilled to present our digital marketing strategy to help [CLIENT_NAME] achieve your business growth goals.",
                    ProblemStatement = "In today's competitive landscape, you need a data-driven marketing approach to reach and convert your target audience effectively.",
                    ProposedSolution = "We'll create and execute a comprehensive digital marketing campaign including social media management, paid advertising, content marketing, and email campaigns.",
                    Methodology = "Our approach: 1) Market Research & Audience Analysis, 2) Strategy Development, 3) Campaign Launch, 4) Ongoing Optimization & Reporting.",
                    Deliverables = "- Marketing strategy document\n- Social media content calendar\n- Ad campaign management\n- Monthly performance reports\n- ROI tracking dashboard",
                    Timeline = "- Month 1: Strategy & Setup\n- Months 2-6: Campaign Execution\n- Ongoing: Optimization",
                    TeamAndExperience = "Our marketing team has driven $10M+ in client revenue through strategic digital campaigns.",
                    TermsAndConditions = "Monthly retainer with 3-month minimum commitment. Ad spend billed separately.",
                    CallToAction = "Let's start growing your business together. Schedule a strategy session to get started!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "Starter", PriceMin = 2000, PriceMax = 3000, Timeline = "Monthly" },
                    Standard = new PricingTierDto { Name = "Growth", PriceMin = 5000, PriceMax = 8000, Timeline = "Monthly" },
                    Premium = new PricingTierDto { Name = "Enterprise", PriceMin = 10000, PriceMax = 20000, Timeline = "Monthly" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 20
            },

            // 3. Graphic Design Template
            new ProposalTemplate
            {
                Name = "Brand Identity Design",
                Description = "Complete brand identity and design system",
                Category = "Design",
                Tags = SerializeTags(new List<string> { "design", "branding", "logo", "identity" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "We're excited to help [CLIENT_NAME] create a memorable brand identity that resonates with your audience.",
                    ProblemStatement = "A strong brand identity is crucial for standing out in your market and building customer loyalty.",
                    ProposedSolution = "We'll develop a complete brand identity including logo design, color palette, typography, and brand guidelines.",
                    Methodology = "1) Brand Discovery, 2) Concept Development, 3) Design Refinement, 4) Brand Guide Creation.",
                    Deliverables = "- Logo design (3 concepts)\n- Color palette\n- Typography system\n- Brand guidelines document\n- Final files in all formats",
                    Timeline = "- Week 1-2: Discovery & Concepts\n- Week 3-4: Refinement\n- Week 5: Finalization",
                    TeamAndExperience = "Our design team has created award-winning brand identities for 200+ companies.",
                    TermsAndConditions = "50% deposit to start. 3 rounds of revisions included.",
                    CallToAction = "Ready to build a brand that stands out? Let's create something amazing together!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "Logo Only", PriceMin = 1500, PriceMax = 3000, Timeline = "2-3 weeks" },
                    Standard = new PricingTierDto { Name = "Brand Identity", PriceMin = 5000, PriceMax = 8000, Timeline = "4-5 weeks" },
                    Premium = new PricingTierDto { Name = "Complete Branding", PriceMin = 10000, PriceMax = 15000, Timeline = "6-8 weeks" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 15
            },

            // 4. Consulting Services Template
            new ProposalTemplate
            {
                Name = "Business Consulting Services",
                Description = "Strategic business consulting and advisory",
                Category = "Consulting",
                Tags = SerializeTags(new List<string> { "consulting", "strategy", "business", "advisory" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "Thank you for the opportunity to support [CLIENT_NAME] with strategic business consulting.",
                    ProblemStatement = "Every business faces unique challenges. You need expert guidance to navigate growth, optimize operations, and achieve your strategic objectives.",
                    ProposedSolution = "We'll provide hands-on consulting to analyze your current state, identify opportunities, and implement solutions that drive measurable results.",
                    Methodology = "1) Current State Assessment, 2) Gap Analysis, 3) Strategy Development, 4) Implementation Support, 5) Performance Monitoring.",
                    Deliverables = "- Comprehensive assessment report\n- Strategic recommendations\n- Implementation roadmap\n- Weekly progress meetings\n- KPI tracking dashboard",
                    Timeline = "- Week 1-2: Assessment\n- Week 3-4: Strategy Development\n- Ongoing: Implementation Support",
                    TeamAndExperience = "Our consultants have 20+ years combined experience helping businesses scale from $1M to $50M+.",
                    TermsAndConditions = "Engagement billed monthly. Minimum 3-month commitment recommended for best results.",
                    CallToAction = "Let's discuss how we can help accelerate your business growth. Schedule a consultation today!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "Advisory", PriceMin = 5000, PriceMax = 8000, Timeline = "Monthly" },
                    Standard = new PricingTierDto { Name = "Strategic Partnership", PriceMin = 12000, PriceMax = 20000, Timeline = "Monthly" },
                    Premium = new PricingTierDto { Name = "Full Engagement", PriceMin = 25000, PriceMax = 50000, Timeline = "Monthly" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 20
            },

            // 5. Content Writing Template
            new ProposalTemplate
            {
                Name = "Content Writing Services",
                Description = "Professional content creation for blogs, websites, and marketing",
                Category = "Writing",
                Tags = SerializeTags(new List<string> { "writing", "content", "copywriting", "blog" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "We're excited to help [CLIENT_NAME] create compelling content that engages your audience and drives results.",
                    ProblemStatement = "Quality content is essential for SEO, audience engagement, and establishing thought leadership in your industry.",
                    ProposedSolution = "We'll create high-quality, SEO-optimized content tailored to your brand voice and marketing objectives.",
                    Methodology = "1) Content Strategy, 2) Research & Outlining, 3) Writing & Editing, 4) SEO Optimization, 5) Revision & Approval.",
                    Deliverables = "- [X] blog posts per month\n- SEO keyword optimization\n- Meta descriptions\n- Image suggestions\n- Content calendar",
                    Timeline = "Delivered weekly/monthly based on package",
                    TeamAndExperience = "Our writers have published 1000+ articles across major publications and industry blogs.",
                    TermsAndConditions = "Monthly retainer. 2 rounds of revisions per piece included.",
                    CallToAction = "Ready to elevate your content strategy? Let's create content that converts!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "4 Posts/Month", PriceMin = 1200, PriceMax = 2000, Timeline = "Monthly" },
                    Standard = new PricingTierDto { Name = "8 Posts/Month", PriceMin = 2200, PriceMax = 3500, Timeline = "Monthly" },
                    Premium = new PricingTierDto { Name = "12 Posts/Month", PriceMin = 4000, PriceMax = 6000, Timeline = "Monthly" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 15
            },

            // 6. SEO Services Template
            new ProposalTemplate
            {
                Name = "SEO Optimization Services",
                Description = "Comprehensive SEO strategy and implementation",
                Category = "SEO",
                Tags = SerializeTags(new List<string> { "seo", "search", "optimization", "ranking" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "We're ready to help [CLIENT_NAME] improve your search engine rankings and drive organic traffic.",
                    ProblemStatement = "Your target customers are searching for your services online. You need to rank higher to capture this valuable organic traffic.",
                    ProposedSolution = "We'll implement a comprehensive SEO strategy including technical optimization, on-page SEO, link building, and content optimization.",
                    Methodology = "1) SEO Audit, 2) Keyword Research, 3) Technical Fixes, 4) On-Page Optimization, 5) Link Building, 6) Monthly Reporting.",
                    Deliverables = "- Complete SEO audit\n- Keyword strategy\n- Technical optimization\n- Monthly ranking reports\n- Ongoing optimization",
                    Timeline = "- Month 1: Audit & Foundation\n- Months 2-6: Implementation & Growth\n- Ongoing: Optimization",
                    TeamAndExperience = "Our SEO specialists have helped clients achieve top 3 rankings for competitive keywords.",
                    TermsAndConditions = "Monthly retainer with 6-month minimum for optimal results.",
                    CallToAction = "Let's improve your search visibility. Start ranking higher today!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "Local SEO", PriceMin = 1500, PriceMax = 2500, Timeline = "Monthly" },
                    Standard = new PricingTierDto { Name = "National SEO", PriceMin = 3500, PriceMax = 6000, Timeline = "Monthly" },
                    Premium = new PricingTierDto { Name = "Enterprise SEO", PriceMin = 8000, PriceMax = 15000, Timeline = "Monthly" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 20
            },

            // 7. Mobile App Development Template
            new ProposalTemplate
            {
                Name = "Mobile App Development",
                Description = "Native or cross-platform mobile application development",
                Category = "Mobile Development",
                Tags = SerializeTags(new List<string> { "mobile", "app", "ios", "android" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "Thank you for considering us to develop your mobile application. We're excited to bring your app idea to life.",
                    ProblemStatement = "[CLIENT_NAME] needs a mobile app that provides exceptional user experience and meets your business objectives.",
                    ProposedSolution = "We'll develop a high-quality mobile application using modern frameworks, ensuring smooth performance across iOS and Android platforms.",
                    Methodology = "1) Requirements & Wireframes, 2) UI/UX Design, 3) Development, 4) Testing & QA, 5) App Store Launch, 6) Post-Launch Support.",
                    Deliverables = "- iOS & Android apps\n- Backend API (if needed)\n- Admin dashboard\n- App store submission\n- Source code & documentation",
                    Timeline = "- Weeks 1-3: Design\n- Weeks 4-12: Development\n- Weeks 13-14: Testing\n- Week 15: Launch",
                    TeamAndExperience = "Our mobile team has launched 50+ apps with millions of downloads.",
                    TermsAndConditions = "33% deposit, 33% at midpoint, 34% upon completion.",
                    CallToAction = "Ready to launch your app? Let's build something users will love!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "MVP App", PriceMin = 15000, PriceMax = 30000, Timeline = "8-12 weeks" },
                    Standard = new PricingTierDto { Name = "Full-Featured App", PriceMin = 40000, PriceMax = 80000, Timeline = "12-16 weeks" },
                    Premium = new PricingTierDto { Name = "Enterprise App", PriceMin = 100000, PriceMax = 200000, Timeline = "16-24 weeks" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 25
            },

            // 8. Social Media Management Template
            new ProposalTemplate
            {
                Name = "Social Media Management",
                Description = "Complete social media strategy and daily management",
                Category = "Social Media",
                Tags = SerializeTags(new List<string> { "social media", "instagram", "facebook", "content" }),
                Content = SerializeContent(new TemplateContentDto
                {
                    Introduction = "We're excited to help [CLIENT_NAME] build a strong social media presence and engage with your audience.",
                    ProblemStatement = "Social media is crucial for brand awareness and customer engagement. You need consistent, strategic content to grow your following.",
                    ProposedSolution = "We'll manage your social media accounts with daily posting, community engagement, and strategic growth tactics.",
                    Methodology = "1) Social Audit, 2) Strategy Development, 3) Content Creation, 4) Daily Management, 5) Analytics & Optimization.",
                    Deliverables = "- [X] posts per week\n- Community management\n- Monthly analytics reports\n- Hashtag research\n- Trend monitoring",
                    Timeline = "Ongoing monthly service with weekly content delivery",
                    TeamAndExperience = "We've grown social accounts from 0 to 100K+ followers through strategic content and engagement.",
                    TermsAndConditions = "Monthly retainer with 3-month minimum commitment.",
                    CallToAction = "Let's grow your social media presence together. Start engaging your audience today!"
                }),
                DefaultPricing = SerializePricing(new PricingTemplateDto
                {
                    Basic = new PricingTierDto { Name = "1 Platform", PriceMin = 800, PriceMax = 1500, Timeline = "Monthly" },
                    Standard = new PricingTierDto { Name = "3 Platforms", PriceMin = 2000, PriceMax = 3500, Timeline = "Monthly" },
                    Premium = new PricingTierDto { Name = "Full Service", PriceMin = 5000, PriceMax = 8000, Timeline = "Monthly" }
                }),
                IsSystemTemplate = true,
                IsPublic = true,
                EstimatedTimeMinutes = 15
            }
        };
    }

    private static string SerializeTags(List<string> tags)
    {
        return JsonSerializer.Serialize(tags);
    }

    private static string SerializeContent(TemplateContentDto content)
    {
        return JsonSerializer.Serialize(content);
    }

    private static string SerializePricing(PricingTemplateDto pricing)
    {
        return JsonSerializer.Serialize(pricing);
    }
}
