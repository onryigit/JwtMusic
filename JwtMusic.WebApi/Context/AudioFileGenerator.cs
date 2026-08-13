namespace JwtMusic.WebApi.Context;

public static class AudioFileGenerator
{
    public static void EnsureCreated(string contentRoot)
    {
        var directory = Path.Combine(contentRoot, "Audio");
        Directory.CreateDirectory(directory);
        for (var track = 1; track <= 20; track++)
        {
            var path = Path.Combine(directory, $"track-{track:00}.mp3");
            if (File.Exists(path)) continue;
            using var output = File.Create(path);
            // MPEG-1 Layer III, 128 kbps, 44.1 kHz demo frames. Audio remains local and copyright-free.
            var frame = new byte[417];
            frame[0] = 0xFF; frame[1] = 0xFB; frame[2] = 0x90; frame[3] = 0x64;
            for (var i = 0; i < 1150; i++) output.Write(frame);
        }
    }
}
