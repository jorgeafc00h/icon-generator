# Icon Generator - Integration Tests

Integration tests for experimenting with prompt engineering and evaluating image generation quality.

## 🎯 Purpose

These tests allow you to:
1. **Experiment** with different prompt strategies
2. **Compare** styles, colors, and quality settings
3. **Evaluate** generated prompts before spending credits
4. **Iterate** on prompt engineering to improve output quality
5. **Document** what works best for different use cases

## 🚀 Setup

### 1. Configure Azure OpenAI Credentials

Set environment variables:

```bash
# Required
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_API_KEY="your-api-key"

# Optional (defaults shown)
export DALLE3_DEPLOYMENT_NAME="dall-e-3"
export GPT4O_MINI_DEPLOYMENT_NAME="gpt-4o-mini"
```

**Or** create a `.env` file in the `Tests` directory:

```env
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_API_KEY=your-api-key-here
DALLE3_DEPLOYMENT_NAME=dall-e-3
GPT4O_MINI_DEPLOYMENT_NAME=gpt-4o-mini
```

### 2. Build the Test Project

```bash
cd /Users/jorgeflores/github/icon-generator/api/Tests
dotnet build
```

### 3. Run Tests

```bash
dotnet run
```

You'll see an interactive menu:

```
Select test to run:
1. Style Variations
2. Color Palette Variations
3. Prompt A/B Testing
4. Quality Comparison
5. Batch Generation
6. Generate and Save Results
7. Run All Tests
```

## 📊 Available Tests

### Test 1: Style Variations
Compare how different styles (3D, minimal, gradient, etc.) affect the generated prompts.

**Use Case:** Find which style works best for your app category.

**Example Output:**
```
--- Style: 3D ---
Enhanced Prompt:
Create a professional 3D rendered fitness tracker app icon. Center a stylized
heart rate monitor with smooth gradients from #FF6B6B to #4ECDC4. Apply soft
lighting from top-left...

Prompt Quality Score: 100.0%
✓ Color Guidance
✓ Composition Rules
✓ Style Guidelines
✓ Quality Constraints
✓ Scale Considerations
```

### Test 2: Color Palette Variations
Test different color combinations to see how they're incorporated into prompts.

**Use Case:** Discover optimal color palettes for your brand.

### Test 3: Prompt A/B Testing
Generate 3 variations of the same concept (literal, abstract, metaphorical).

**Use Case:** Explore creative interpretations before generating images.

### Test 4: Quality Comparison
Compare "standard" vs "hd" quality prompt generation.

**Use Case:** Understand quality differences and cost tradeoffs.

### Test 5: Batch Generation
Test multiple icon concepts in one run.

**Use Case:** Evaluate consistency across different keywords.

### Test 6: Generate and Save Results
Save all results to a markdown file for review and comparison.

**Use Case:** Document experiments and share with team.

**Output Location:** `./test-results/prompt-results-[timestamp].md`

## 💡 Recommended Workflow

### Phase 1: Exploration (Free - No Image Generation)
```bash
# Run tests 1-5 to experiment with prompts
# This only uses GPT-4o-mini (cheap: ~$0.01 per 100 prompts)
dotnet run
> Select: 1, 2, 3, 4, or 5
```

### Phase 2: Documentation
```bash
# Save your best findings
dotnet run
> Select: 6
# Review: ./test-results/prompt-results-*.md
```

### Phase 3: Image Generation (Costs Money)
Once you're happy with prompts, uncomment the image generation lines in the test code:

```csharp
// In TestStyleVariations() method, uncomment:
var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, request.Quality);
Console.WriteLine($"Generated Image: {imageUrl}\n");
```

**Cost Estimate:**
- Standard: $0.040 per image
- HD: $0.080 per image

## 🎨 Customizing Tests

### Add Your Own Test Cases

Edit `PromptExperimentationTests.cs`:

```csharp
public async Task TestMyApp()
{
    var request = new IconGenerationRequest
    {
        Keywords = "your app concept",
        Style = "3D", // or minimal, gradient, etc.
        Colors = new List<string> { "#YOUR_COLOR", "#YOUR_COLOR" },
        Quality = "hd"
    };

    var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
    Console.WriteLine($"Prompt: {enhancedPrompt}");
}
```

### Test New Styles

Add style to `DesignKnowledgeBase.cs`:

```csharp
public const string YourNewStyle = @"
YOUR_STYLE GUIDELINES:
- Specific visual characteristics
- Color treatment
- Composition rules
- Reference inspiration
";
```

## 📈 Interpreting Results

### Quality Score Breakdown

```
Prompt Quality Score: 100.0%
✓ Color Guidance - Uses specified colors appropriately
✓ Composition Rules - Mentions layout, balance, hierarchy
✓ Style Guidelines - References the chosen style
✓ Quality Constraints - Includes "professional", "scalable"
✓ Scale Considerations - Mentions different icon sizes
```

**90-100%:** Excellent - Ready for image generation
**70-90%:** Good - May need minor tweaks
**Below 70%:** Review and enhance manually

### Prompt Analysis

Good prompts should:
1. ✅ Be **specific** about visual elements
2. ✅ Include **color guidance**
3. ✅ Reference **composition rules**
4. ✅ Mention **scale/platform requirements**
5. ✅ Avoid **text or letters**
6. ✅ Have **clear focal point**

## 🔍 Troubleshooting

### Error: "Azure OpenAI credentials not found"
- Check environment variables are set
- Verify `.env` file exists and is formatted correctly

### Error: "DeploymentNotFound"
- Verify your deployment names match what's in Azure AI Foundry
- Check `DALLE3_DEPLOYMENT_NAME` and `GPT4O_MINI_DEPLOYMENT_NAME`

### Prompts seem generic
- Check that `PromptEngineeringService` is registered in DI
- Verify design knowledge base is being loaded
- Try different styles and keywords

## 📚 Next Steps

1. **Run all tests** to understand baseline quality
2. **Document findings** using Test 6
3. **Iterate on prompts** that score below 90%
4. **Generate actual images** for top 3-5 prompts
5. **Refine knowledge base** based on results

## 💰 Cost Management

**Prompt Enhancement (GPT-4o-mini):**
- ~$0.0001 per prompt
- 1000 prompts ≈ $0.10

**Image Generation (DALL-E 3):**
- Standard: $0.040 per image
- HD: $0.080 per image
- 100 images ≈ $4-8

**Recommendation:** Generate 100+ prompts first (< $1), then generate 10-20 best images ($0.40-$1.60)

---

Happy experimenting! 🎨✨
