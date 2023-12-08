using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PlayAccordionVisualizer
{

    public partial class Form1 : Form
    {
        private Dictionary<int, Button> keyboard;
        private MidiFile midiFile;
        private MidiEventCollection events;
        private int eventIndex = 0;
        private MidiOut midiOut;

        public Form1()
        {
            InitializeComponent();

            // Luo näppäimistö
            keyboard = new Dictionary<int, Button>
        {

            { 95, CreateButton("B", 845, 95, Color.White, "White") },
{ 94, CreateButton("A#", 755, 95, Color.Black, "Black") },
{ 93, CreateButton("A", 800, 120, Color.White, "White") },
{ 92, CreateButton("G#", 845, 145, Color.Black, "Black") },
{ 91, CreateButton("G", 755, 145, Color.White, "White") },
{ 90, CreateButton("F#", 800, 170, Color.Black, "Black") },
{ 89, CreateButton("F", 845, 195, Color.White, "White") },
{ 88, CreateButton("E", 755, 195, Color.White, "White") },
{ 87, CreateButton("D#",800, 220, Color.Black, "Black") },
{ 86, CreateButton("D", 845, 245, Color.White, "White") },
{ 85, CreateButton("C#",755, 245, Color.Black, "Black") },
{ 84, CreateButton("C", 800, 270, Color.White, "White") },

{ 83, CreateButton("B", 845, 295, Color.White, "White") },
{ 82, CreateButton("A#", 755, 295, Color.Black, "Black") },
{ 81, CreateButton("A", 800, 320, Color.White, "White") },
{ 80, CreateButton("G#", 845, 345, Color.Black, "Black") },
{ 79, CreateButton("G", 755, 345, Color.White, "White") },
{ 78, CreateButton("F#", 800, 370, Color.Black, "Black") },
{ 77, CreateButton("F", 845, 395, Color.White, "White") },
{ 76, CreateButton("E", 755, 395, Color.White, "White") },
{ 75, CreateButton("D#",800, 420, Color.Black, "Black") },
{ 74, CreateButton("D", 845, 445, Color.White, "White") },
{ 73, CreateButton("C#",755, 445, Color.Black, "Black") },
{ 72, CreateButton("C", 800, 470, Color.White, "White") },

{ 71, CreateButton("B", 845, 495, Color.White, "White") },
{ 70, CreateButton("A#", 755, 495, Color.Black, "Black") },
{ 69, CreateButton("A", 800, 520, Color.White, "White") },
{ 68, CreateButton("G#", 845, 545, Color.Black, "Black") },
{ 67, CreateButton("G", 755, 545, Color.White, "White") },
{ 66, CreateButton("F#", 800, 570, Color.Black, "Black") },
{ 65, CreateButton("F", 845, 595, Color.White, "White") },
{ 64, CreateButton("E", 755, 595, Color.White, "White") },
{ 63, CreateButton("D#",800, 620, Color.Black, "Black") },
{ 62, CreateButton("D", 845, 645, Color.White, "White") },
{ 61, CreateButton("C#",755, 645, Color.Black, "Black") },
{ 60, CreateButton("C", 800, 670, Color.White, "White") }

        };

            foreach (var key in keyboard.Values)
            {
                Controls.Add(key);
            }

            // Lataa MIDI-tiedosto
            midiFile = new MidiFile("test.mid");
            events = midiFile.Events;

            // Luo MIDI-lähtö
            midiOut = new MidiOut(0);
        }

        private Button CreateButton(string text, int x, int y, Color color, string tag)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                BackColor = color,
                ForeColor = tag == "Black" ? Color.White : Color.Black,  // Määritetään tekstiväri
                FlatStyle = FlatStyle.Flat,
                Width = 50,
                Height = 50,
                Tag = tag
            };

            // Poista reuna
            button.FlatAppearance.BorderSize = 0;

            // Tee näppäimestä pyöreä
            var path = new GraphicsPath();
            path.AddEllipse(0, 0, button.Width, button.Height);
            button.Region = new Region(path);

            return button;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Aloita MIDI-tiedoston toisto toisessa säikeessä
            Task.Run(() => PlayMidiFile());
        }

        private void PlayMidiFile()
        {
            foreach (var midiEvent in events[0])
            {
                if (midiEvent is NoteOnEvent noteOnEvent)
                {
                    if (keyboard.TryGetValue(noteOnEvent.NoteNumber, out var key))
                    {
                        // Päivitä näppäimen väri pääsäikeessä
                        Invoke((Action)(() => key.BackColor = noteOnEvent.Velocity > 0 ? Color.Blue : key.Tag.ToString() == "Black" ? Color.Black : SystemColors.Control));
                        midiOut.Send(midiEvent.GetAsShortMessage());
                    }
                }
                else if (midiEvent is NoteEvent noteOffEvent)
                {
                    if (keyboard.TryGetValue(noteOffEvent.NoteNumber, out var key))
                    {
                        // Päivitä näppäimen väri pääsäikeessä
                        Invoke((Action)(() => key.BackColor = key.Tag.ToString() == "Black" ? Color.Black : SystemColors.Control));
                        midiOut.Send(midiEvent.GetAsShortMessage());
                    }
                }

                // Odota seuraavaan tapahtumaan
                if (eventIndex + 1 < events[0].Count)
                {
                    var nextEvent = events[0][eventIndex + 1];
                    var delay = (nextEvent.AbsoluteTime - midiEvent.AbsoluteTime) * 250 / midiFile.DeltaTicksPerQuarterNote;
                    System.Threading.Thread.Sleep((int)delay);
                }

                eventIndex++;
            }
        }

    }

}