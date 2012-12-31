module RakeHelper
  def RakeHelper.xunit_command(version)
    return use_mono ? File.dirname(__FILE__) + "/xunit.console.mono." + version.to_s + "." + script_extension : ENV["XunitConsole_" + version.to_s]
  end

  def RakeHelper.nuget_command
    return use_mono ? File.dirname(__FILE__) + "/Nuget.Mono." + script_extension : ENV["NuGetConsole"]
  end

  def RakeHelper.use_mono
    return !is_windows || ENV["Mono"]
  end
  
  private
  def RakeHelper.script_extension
    return is_windows ? "bat" : "sh"
  end

  def RakeHelper.is_windows
    return ENV["OS"] == "Windows_NT"
  end
end
