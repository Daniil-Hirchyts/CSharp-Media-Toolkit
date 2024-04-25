# MediaTools class library

## How to use it

    To test each function, we have a class **Launch** that only contains a *Main* function. This way, we can execute the program using command lines.
    All functions to test are commented between lines of "//", we just need to uncomment them INDIVIDUALLY to test them.
    Each function needs inputs to write in command line, depending on what function we want to test. The first argument is always the path to the file to analyse.
    The others are specified for each function in a try/except block before the call of the function. 
    (TODO: parser that will gather all this)

## Classes

### AudioTools

#### string ConvertToAudio(string input_filename, <br> string extension = "wav", <br> string outdir = @"./Data/Audios/")

    Converts any audio or video file to a specified extension, using a Ffmpeg process.
    Returns a path to the created file.

    Parameters:
         - string input_filename: Path to the audio or video file to convert
         - string extension: (Optionnal) Extension of the output audio or video file. Default "wav".
         - string outdir: (Optionnal) Path of the folder to save the created file.
        

#### void WaveForm(string file, bool ismixed = true, int height = 400, int width = 600, string color = "brown", bool is_create_csv = false, bool overwrite_csv = false, string outdir = @"./WaveForm/PNGs/")
        
    Creates a WaveForm of the audio file into a png file, using a Ffmpeg process.

    Parameters:
        - string file: Path to the audio or video file.
        - bool ismixed: (Optional) True if all channels have to be downmixed to one. Default true.
        - int height: (Optional) Height of the png result. Default 400.
        - int width: (Optional) Width of the png result. Default 600.
        - string color: (Optional) Color of the waveform. Default "brown".
        - bool is_create_csv: (Optional) Boolean. If true, decodes all audio values (too long). Default false.
        - bool overwrite_csv: (Optional) Boolean. If true, overwrite the decoded audio values if they already exist (too long). Default false.
        - outdir: (Optional) Path of the folder to save the created png file.


### VideoTools

#### void ShotDetection(string filename, string threshold, uint scale = 500, string outdir = @"./ShotDetection/JPGs/")

    Detects shot changes in a video, and computes jpg images for each change, using a Ffmpeg process.

    Parameters:
        - filename: Path to the audio or video file.
        - threshold: Percenatage threshod of the difference between two consecutives frames.
        - scale: (Optional) Scale of the created jpg images. Default 500.
        - outdir: (Optional) Path of the folder to save the created jpg file.

#### List<float> ShotTimeStamps(string filename)

    Extracts time stamps from the metadata file containing informations on the shot changes.
    Returns a list of floats representing the time stamps.

    Parameters:
        - filename: Path to the file containing metadata.
        

### SpeechToText

#### SpeechToText(RegionEndpoint regionEndpoint = null)

    Initialization of the SpeechToText class object.
       
    Parameters
        - RegionEndpoint regionEndpoint: (Optionnal) Default EUWest3.

#### void Dispose()

    Dispose the SpeechToText object. Remember to call !


#### async Task<ResponseJSON[]> TranscribeInputFile(string fileName, string targetLanguageCode = "fr-FR", string bucketName = "", string outdir = "./AmazonSpeechToText/", bool diarization = true, int MaxSpeakers = 2)
        
    Asynchronous Transcription of an audio with Amazon API. Create a Json file containing all the information of the transcription.
    
    Parameters:
        - string fileName: Path to the video or audio file to transcribe. Must be in format FLAC, MP3, MP4, Ogg, WebM, AMR or WAV.
        - string targetLanguageCode: (Optionnal) Language of the audio in the format "en-US". Default "fr-FR".
        - string bucketName: (Optionnal) Name of the bucket if specified. Default "".
        - string outdir: (Optionnal) Path to the folder where to save the results. Default "./AmazonSpeechToText/".
        - string diarization: (Optionnal) True if the API should separate the different speakers. Default true.
        - int MaxSpeakers: (Optionnal) Maximum number of speakers to recognize in the audio. Default 2.

#### async Task<PutBucketResponse> CreateBucket(string bucketName)

    Asynchronous creation of a bucket named bucketname on the Amazon S3 server. 

    Parameters:
        - string bucketName: Name of the bucket to create.

#### async Task<PutObjectResponse> UploadInputFileToS3(string fileName, string bucketName)
      
    Asynchronously upload the file *fileName* into the bucket *bucketName* on the Amazon S3 storage.

    Parameters:
        - string fileName: Path to the file to upload.
        - string bucketName: Name of the bucket.

#### async Task<string> GetFileFromS3(string keyName, string bucketName)

    Asynchronously get the file with the key *keyName* from the bucket *bucketName* on the Amazon S3 storage.

    Parameters:
        - string keyName: Key of the file to get.
        - string bucketName: Name of the bucket.
       
#### ResponseJSON DeserializeJSON(string json_string)

    Deserialize a JSON string into a C# class ResponseJSON object.
    (A JSON string is the string extracted from a JSON file).

    Parameters:
        - string json_string: path to the JSON file.

#### string SerializeJSON(ResponseJSON resp)

    Serialize a JSON file from a C# class ResponseJSON object into a JSON string.

    Parameters:
        - ResponseJSON resp: C# ResponseJSON object that represents a transcription response from Amazon Transcribe API.

#### ResponseJSON[] DiarizationResponse(ResponseJSON response)

    From a transcription response containing *n* speakers, this functions creates *n* ResponseJSON objects representing the transcription results for each speaker.

    Parameters:
        - ResponseJSON response: Response from the Amazon Transcribe API.

#### static void CreateXMLFromJSON(SpeechToText.ResponseJSON resp, string filename, string language, string shot_folder, string outdir = "./AmazonSpeechToText/")

     Converter from ResponseJSON to XML.
      
     Parameters:
        - ResponseJSON resp: Response of the API converted into a C# class.
        - string name: Name of the file.
        - string outdir: Directory to store the XML file.
         

### UseTools

#### void CreateDirectory(string path)
     
    Create a directory if it does not already exist. The path is either relative ("./...") or absolute (C://...)
    
    Parameters:
        - path: Path to create.


#### string GetExtension(string filename)

     Computes the extension of the filename.
     
     Parameters:
        filename: Path to the file.


#### string[] GetNameAndExtension(string filename)

     Computes the extension of the filename and returns also the name part.
     
     Parameters:
        filename: Path to the file.
        