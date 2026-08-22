
namespace Kyanite.Dialog;

internal class DialogBuilder
{
     string title = "Dialog";
     int width = 300, height = 200;
     
     public DialogBuilder WithTitle(string title)
     {
          this.title = title;
          return this;
     }

     public DialogBuilder WithSize(int width, int height)
     {
          this.width = width;
          this.height = height;
          return this;
     }

     public Dialog Build()
     {
          return new Dialog(title, width, height);
     }
}
