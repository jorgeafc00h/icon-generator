# Integration Tests - Quick Start Guide

Get started with prompt experimentation in 5 minutes!

## 📋 Prerequisites

Before you begin, make sure you have:
- ✅ Completed Azure AI Foundry setup (see `Docs/AZURE_AI_FOUNDRY_SETUP.md`)
- ✅ Azure OpenAI endpoint and API key
- ✅ DALL-E 3 and GPT-4o-mini models deployed

## 🚀 Quick Start (5 Minutes)

### Step 1: Navigate to Tests Directory

```bash
cd /Users/jorgeflores/github/icon-generator/api/Tests
```

### Step 2: Run Setup Script

```bash
./setup-tests.sh
```

This interactive script will:
- Prompt you for your Azure credentials
- Save them to `.env` file
- Validate the configuration
- Optionally test connectivity

**Example:**
```
Azure OpenAI Endpoint: https://your-resource.openai.azure.com/
Azure OpenAI API Key: abc123def456...
DALL-E 3 Deployment Name: dall-e-3
GPT-4o-mini Deployment Name: gpt-4o-mini
```

### Step 3: Load Environment Variables

```bash
source .env
```

### Step 4: Build the Tests

```bash
dotnet build
```

### Step 5: Run Tests

```bash
dotnet run
```

You'll see a menu:
```
===========================================
  Icon Generator - Prompt Experimentation
===========================================

Select test to run:
1. Style Variations
2. Color Palette Variations
3. Prompt A/B Testing
4. Quality Comparison
5. Batch Generation
6. Generate and Save Results
7. Run All Tests

Enter choice (1-7):
```

### Step 6: Start with Test #1

Type `1` and press Enter.

**What happens:**
- Tests 6 different styles (3D, minimal, gradient, etc.)
- Shows enhanced prompts for each
- Displays quality scores
- NO image generation (saves money!)

**Example Output:**
```
--- Style: 3D ---
Enhanced Prompt:
Create a professional 3D rendered fitness tracker app icon. Center a stylized
heart rate monitor with smooth gradients from vibrant coral (#FF6B6B) to
turquoise (#4ECDC4). Apply soft studio lighting from top-left at 45°...

Prompt Quality Score: 100.0%
✓ Color Guidance
✓ Composition Rules
✓ Style Guidelines
✓ Quality Constraints
✓ Scale Considerations
```

---

## 📊 What Each Test Does

### Test 1: Style Variations ⭐ START HERE
**What:** Compares 6 different styles for the same concept
**Cost:** ~$0.001 (6 prompts)
**Time:** ~30 seconds
**Use:** Find which style works best for your app category

### Test 2: Color Palette Variations
**What:** Tests 5 different color combinations
**Cost:** ~$0.001 (5 prompts)
**Time:** ~30 seconds
**Use:** Discover optimal color palettes

### Test 3: Prompt A/B Testing
**What:** Generates 3 variations (literal, abstract, metaphorical)
**Cost:** ~$0.0003 (3 prompts)
**Time:** ~15 seconds
**Use:** Explore creative interpretations

### Test 4: Quality Comparison
**What:** Compares "standard" vs "hd" quality prompts
**Cost:** ~$0.0002 (2 prompts)
**Time:** ~10 seconds
**Use:** Understand quality differences

### Test 5: Batch Generation
**What:** Tests 5 different icon concepts
**Cost:** ~$0.001 (5 prompts)
**Time:** ~30 seconds
**Use:** Evaluate consistency across concepts

### Test 6: Generate and Save Results ⭐ DOCUMENT
**What:** Tests 3 concepts and saves detailed results to file
**Cost:** ~$0.0003 (3 prompts)
**Time:** ~15 seconds
**Output:** `./test-results/prompt-results-[timestamp].md`
**Use:** Document your findings

---

## 💡 Recommended Workflow

### Phase 1: Initial Exploration (5-10 minutes)
```bash
dotnet run
> Enter: 1  # Style variations
> Enter: 2  # Color palettes
> Enter: 6  # Save results
```

**Review:** Check `./test-results/prompt-results-*.md`

### Phase 2: Refinement (10-15 minutes)
Based on your findings:
1. Edit `api/Prompts/DesignKnowledgeBase.cs`
2. Adjust style guidelines
3. Run tests again
4. Compare results

### Phase 3: Image Generation (when ready)
Uncomment in test code:
```csharp
// var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, request.Quality);
// Console.WriteLine($"Generated Image: {imageUrl}\n");
```

Run selected tests to generate actual images.

---

## 🎨 Customizing Tests

### Test Your Own Concepts

Edit `PromptExperimentationTests.cs`:

```csharp
public async Task TestMyApp()
{
    var request = new IconGenerationRequest
    {
        Keywords = "your app name or concept",
        Style = "3D",  // or: minimal, gradient, glassmorphism, clay, pixel
        Colors = new List<string> { "#FF6B6B", "#4ECDC4" },  // or empty for AI choice
        Quality = "hd"  // or: standard
    };

    var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
    Console.WriteLine($"\nEnhanced Prompt:\n{enhancedPrompt}\n");

    var qualityScore = _promptService.AnalyzePromptQuality(enhancedPrompt);
    Console.WriteLine(qualityScore.ToString());
}
```

Then in `Main()`:
```csharp
case "8":
    await tests.TestMyApp();
    break;
```

---

## 📈 Understanding Results

### Quality Score Breakdown

```
Prompt Quality Score: 85.0%
✓ Color Guidance - Mentions colors appropriately
✓ Composition Rules - References layout/hierarchy
✗ Style Guidelines - Missing style-specific details
✓ Quality Constraints - Includes "professional"
✓ Scale Considerations - Mentions scalability
```

**90-100%:** Excellent - Ready for image generation ✅
**70-90%:** Good - Minor improvements possible ⚠️
**Below 70%:** Needs work - Review prompt manually ❌

### What Makes a Good Prompt?

✅ **Specific visual details** - "stylized heart rate monitor" not just "icon"
✅ **Color guidance** - Exact hex codes or harmonious descriptions
✅ **Composition rules** - "centered", "rule of thirds", "60% of space"
✅ **Platform requirements** - "iOS guidelines", "scalable 29px-1024px"
✅ **Quality constraints** - "professional", "HD rendering", "clean"
✅ **Style adherence** - "soft studio lighting at 45°" for 3D
✅ **No text** - Explicitly states "no text or letters"

---

## 💰 Cost Tracking

All tests use **GPT-4o-mini only** (no image generation):

| Test | Prompts | Cost Each | Total |
|------|---------|-----------|-------|
| Test 1 | 6 | $0.0001 | ~$0.0006 |
| Test 2 | 5 | $0.0001 | ~$0.0005 |
| Test 3 | 3 | $0.0001 | ~$0.0003 |
| Test 4 | 2 | $0.0001 | ~$0.0002 |
| Test 5 | 5 | $0.0001 | ~$0.0005 |
| Test 6 | 3 | $0.0001 | ~$0.0003 |
| All | 24 | $0.0001 | **~$0.0024** |

**Running all tests 100 times:** ~$0.24 (negligible!)

**Image generation** (when enabled):
- Standard: $0.040 per image
- HD: $0.080 per image

💡 **Pro Tip:** Test 1000 prompts (~$1) before generating any images

---

## 🐛 Troubleshooting

### Error: "Azure OpenAI credentials not found"

**Fix:**
```bash
# Make sure you've sourced the .env file
source .env

# Or export manually
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_API_KEY="your-key"
export DALLE3_DEPLOYMENT_NAME="dall-e-3"
export GPT4O_MINI_DEPLOYMENT_NAME="gpt-4o-mini"
```

### Error: "DeploymentNotFound"

**Fix:** Check deployment names match what's in Azure AI Foundry
```bash
# List deployments
az cognitiveservices account deployment list \
  --name your-openai-resource-name \
  --resource-group rg-icon-generator
```

### Error: "Unauthorized" or "Access Denied"

**Fix:** Verify API key is correct
```bash
# Get your API key
az cognitiveservices account keys list \
  --name your-openai-resource-name \
  --resource-group rg-icon-generator
```

### Prompts seem too generic

**Fix:** Check that `PromptEngineeringService` is being used
- Verify you rebuilt after changes
- Check logs for "Enhanced prompt generated"
- Try different styles/keywords

### Test runs but no output

**Fix:** Check console output level
```bash
# Run with verbose logging
DOTNET_LOGGING_LEVEL=Debug dotnet run
```

---

## 🎯 Next Steps After Testing

1. **Review Results**
   - Check `./test-results/*.md` files
   - Identify which styles work best
   - Note which color palettes are most effective

2. **Refine Knowledge Base**
   - Edit `api/Prompts/DesignKnowledgeBase.cs`
   - Add new style templates
   - Update color palettes

3. **Generate Sample Images**
   - Uncomment image generation
   - Test top 5-10 prompts
   - Save best results

4. **Document Findings**
   - What works well?
   - What needs improvement?
   - Platform-specific considerations?

5. **Iterate**
   - Refine based on actual generated images
   - A/B test variations
   - Build your prompt library

---

## 📚 Additional Resources

- **Full Strategy:** `Docs/PROMPT_ENGINEERING_STRATEGY.md`
- **Azure Setup:** `Docs/AZURE_AI_FOUNDRY_SETUP.md`
- **Test Details:** `api/Tests/README.md`
- **Design Knowledge:** `api/Prompts/DesignKnowledgeBase.cs`

---

**Happy Testing! 🎨✨**

Need help? The quality scores will guide you to better prompts!
