using System.Text;

namespace WeihanLi.Common.Helpers;

public sealed class DelegateTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    private readonly Action<string> _onLineWritten;
    private readonly StringBuilder _builder;

    public DelegateTextWriter(Action<string> onLineWritten)
    {
        _onLineWritten = Guard.NotNull(onLineWritten);

        _builder = new StringBuilder();
    }

    public override void Flush()
    {
        if (_builder.Length > 0)
        {
            FlushInternal();
        }
    }

    public override Task FlushAsync()
    {
        Flush();
        return Task.CompletedTask;
    }

    public override void Write(char value)
    {
        if (value == '\n')
        {
            FlushInternal();
        }
        else
        {
            _builder.Append(value);
        }
    }

    public override Task WriteAsync(char value)
    {
        if (value == '\n')
        {
            FlushInternal();
        }
        else
        {
            _builder.Append(value);
        }
        return Task.CompletedTask;
    }

    private void FlushInternal()
    {
        _onLineWritten(_builder.ToString());
        _builder.Clear();
    }
}
