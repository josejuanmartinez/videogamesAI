# LLM-based generation of content with Mistral 7B (CPU only)
This is a Visual Studio solution for showcasing how you can integrate LLMs to add content to your video game.
No GPU is required, as we will be using quantized models to 3 or 5 bits.

## 1. Download this solution
## 2. Open and look for NuGet plugin
As you will need to install LlamaSharp from NuGet, you need NuGet for Unity to manage the downloading and installation.
The plugin is alredy installed by default, and you should see this:
<img width="919" alt="image" src="https://github.com/josejuanmartinez/videogamesAI/assets/36634572/9285b4a9-d3b0-4f90-b1bf-1b5ab21e0533">

But if not, just click on the .package you will see in `AddToProjects`
<img width="907" alt="image" src="https://github.com/josejuanmartinez/videogamesAI/assets/36634572/a9ffb47c-5361-4ab3-a4d3-dae32f450c3e">

## 3. Go to NuGet -> Manage NuGet Packages
![image](https://github.com/josejuanmartinez/videogamesAI/assets/36634572/11f011c4-80da-456c-8b57-6099c6c07f8f)

Check if LlamaSharp is installed.
![image](https://github.com/josejuanmartinez/videogamesAI/assets/36634572/5e6067f2-b85b-4d88-b029-552c084822a6)

If not, go to `NuGet -> Manage Nu Get Packages -> Online`, type `LlamaSharp` and install it.
![image](https://github.com/josejuanmartinez/videogamesAI/assets/36634572/a3ef99da-b75b-4528-91be-c05429852dc7)

## 4. Open Assets/_Scripts/ContentGenerator.cs, check and look for UniTask
UniTask is a library to get full multithread in Unity.

The library should have been preinstalled for you in the solution. Check that the compiler does not complain about lñacUniTask library.
If it complains about it being missing, then install it the same way as with NuGet, just click on the .package you will see in `AddToProjects`.
<img width="910" alt="image" src="https://github.com/josejuanmartinez/videogamesAI/assets/36634572/e39a928d-fb4b-4b47-b6b5-07d5a7ba51a2">

## 5. Download Mistral 7B weights
In the demo I use two quantized 7B versions. Let's download both of them.

Go to:
https://huggingface.co/TheBloke/Mistral-7B-Instruct-v0.1-GGUF

In there, look for:
<img width="515" alt="image" src="https://github.com/josejuanmartinez/videogamesAI/assets/36634572/3898bf6d-cf32-4764-9a3b-e0e76f8bb3f8">

You will need to click in the model names and then, in the opening tab, download them clicking here:
<img width="878" alt="image" src="https://github.com/josejuanmartinez/videogamesAI/assets/36634572/0956099d-f6cc-4075-b21b-0f3e069dfd29">

Proceed as mentioned for the two models.

## 6. Place the models inside your StreamingAssets/ folder.
It should look like this:
![image](https://github.com/josejuanmartinez/videogamesAI/assets/36634572/322eb9c5-a1bb-4355-896e-ddb8e9bb3cb1)

## 7. Click on Play
It takes some seconds to load the model into memory. After, you can type concepts and click on Submit to see what the model generates.
![image](https://github.com/josejuanmartinez/videogamesAI/assets/36634572/d1dcdaa8-dfb9-4e08-a9e4-20fdb7b00e59)

## 8. Stop, play with the hyperparams and run again. 
Do you see the difference?

![image](https://github.com/josejuanmartinez/videogamesAI/assets/36634572/212ca885-28a2-48c6-a08c-7530e3927d43)

## Additional:
- Feel free to download other GGUF-based LLM models supported by [LlamaSharp](https://github.com/SciSharp/LLamaSharp), including Llama2 and Mistral.
- Do you want to run it in GPU? Let me know!

## Thanks to @eublefar for the [inspiration](https://github.com/eublefar/LLAMASharpUnityDemo)https://github.com/eublefar/LLAMASharpUnityDemo).
