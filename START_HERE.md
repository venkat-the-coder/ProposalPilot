# ProposalPilot AI - Claude Code Initial Prompt

> Copy and paste this prompt to Claude Code to start development.

---

## PROMPT TO GIVE CLAUDE CODE

```
I'm building ProposalPilot AI - an AI-powered SaaS application for freelancers and agencies to create winning proposals.

Please read these files in order:
1. CLAUDE.md - Project overview and coding standards
2. PROJECTPLAN.md - Day-by-day development tasks
3. TECHNICAL_SPECS.md - Database schema and API endpoints
4. AI_PROMPTS.md - Claude AI prompts for the application
5. SETUP_COMMANDS.md - CLI commands reference

After reading these files:

1. Create a GitHub repository named "ProposalPilot"
2. Start with "Day 1 Morning: Create Solution Structure" from PROJECTPLAN.md
3. Follow the tasks sequentially, checking off each one as you complete it
4. After completing each day's tasks, commit with a descriptive message
5. Test that the solution builds before moving to the next day

Current status: Pre-Development complete, ready to start Phase 1 Day 1.

Let's begin!
```

---

## ALTERNATIVE: Start from a Specific Day

If you need to start from a specific day or task:

```
I'm building ProposalPilot AI. Please read CLAUDE.md, PROJECTPLAN.md, TECHNICAL_SPECS.md, and AI_PROMPTS.md.

Current status: [Describe where you are]

Continue from [Day X: Task Name] in PROJECTPLAN.md.
```

---

## TIPS FOR WORKING WITH CLAUDE CODE

### 1. Give Context First
Always point Claude Code to the documentation files before asking it to code.

### 2. Work in Small Chunks
Instead of saying "build the whole app", say "complete Day 1 tasks".

### 3. Verify Before Moving On
After each day, ask Claude Code to:
- Run `dotnet build` to verify it compiles
- Run `ng serve` to verify Angular works (when applicable)
- Commit the changes

### 4. If Something Breaks
```
There's an error: [paste error]
Please fix it before continuing.
```

### 5. Resume After a Break
```
I'm continuing work on ProposalPilot. 
Read PROJECTPLAN.md to see current progress (checked items).
Continue from the next unchecked task.
```

---

## COMMON PROMPTS YOU'LL USE

### Check Progress
```
Read PROJECTPLAN.md and summarize what's complete and what's next.
```

### Run a Specific Day
```
Complete all Day 5 tasks from PROJECTPLAN.md.
```

### Fix an Issue
```
The build is failing with this error: [error]
Fix it and verify the solution builds.
```

### Add Tests
```
Add unit tests for [ServiceName] following the patterns in PROJECTPLAN.md.
```

### Generate a Feature
```
Implement the [FeatureName] feature as described in PROJECTPLAN.md Day X.
Reference TECHNICAL_SPECS.md for the database schema and API endpoints.
Reference AI_PROMPTS.md for any AI integration.
```

---

## ENVIRONMENT VARIABLES

Before starting, ensure these are available (you can set them up as you go):

```bash
# Required for Phase 1-2
DB_CONNECTION_STRING=Server=localhost;Database=ProposalPilotDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True
JWT_SECRET=your-256-bit-secret-key-minimum-32-chars
JWT_ISSUER=ProposalPilot
JWT_AUDIENCE=ProposalPilotUsers

# Required for Phase 2 (AI Features)
ANTHROPIC_API_KEY=sk-ant-xxxxx

# Required for Phase 3 (Payments)
STRIPE_SECRET_KEY=sk_test_xxxxx
STRIPE_PUBLISHABLE_KEY=pk_test_xxxxx

# Required for Phase 4 (Email)
SENDGRID_API_KEY=SG.xxxxx
```

You can add these as you reach each phase.

---

## EXPECTED TIMELINE

| Phase | Days | Focus |
|-------|------|-------|
| Phase 1 | Day 1-10 | Solution structure, Auth, Database |
| Phase 2 | Day 11-20 | Claude AI integration, Brief Analyzer, Proposal Generator |
| Phase 3 | Day 21-30 | Editor, PDF Export, Stripe Payments |
| Phase 4 | Day 31-40 | Email, Tracking, Follow-ups |
| Phase 5 | Day 41-50 | Analytics, Testing, Deployment |

---

## READY TO START?

1. Open Claude Code
2. Navigate to your project directory
3. Paste the initial prompt above
4. Let Claude Code read the files and begin!

Good luck building! 🚀
